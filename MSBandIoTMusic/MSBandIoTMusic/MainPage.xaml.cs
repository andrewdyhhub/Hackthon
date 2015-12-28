using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=391641 上有介绍

namespace MSBandIoTMusic
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        string music1 = "ms-appx:///Assets/Media/ring001.wma";
        string music2 = "ms-appx:///Assets/Media/ring002.wma";
        string music3 = "ms-appx:///Assets/Media/ring003.wma";

        string musicName1 = "像梦一样自由";
        string musicName2 = "你是我的小苹果";
        string musicName3 = "我只在乎你";

        string alertAdjustMusic = "根据您的心率正在为您调整歌曲,即将播放 :";

        //point of changing..
        int low_point = 70;
        int high_point = 78;

        int state = 1;

        bool isWorn = false;

        MSBandModel hrm = null;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。
        /// 此参数通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: 准备此处显示的页面。

            // TODO: 如果您的应用程序包含多个页面，请确保
            // 通过注册以下事件来处理硬件“后退”按钮:
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed 事件。
            // 如果使用由某些模板提供的 NavigationHelper，
            // 则系统会为您处理该事件。
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            if (beatButton.Content.Equals("Connect"))
            {

                if (App.MSBAND != null)
                {
                    await App.MSBAND.StopListening();
                }
                App.MSBAND = new MSBandModel();
                var obj = await App.MSBAND.StartListening();
                if (obj)
                {
                    connectStatusInfo.Text = "已连接";
                }
            }

            hrm = App.MSBAND;
            hrm.BandWornStateReceived += (a, p) =>
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    switch (p)
                    {
                        case Microsoft.Band.Sensors.BandContactState.NotWorn:
                            connectStatusInfo.Text = "请带上手环";
                            isWorn = false;
                            break;

                        case Microsoft.Band.Sensors.BandContactState.Worn:
                            connectStatusInfo.Text = "连接正常";
                            isWorn = true;
                            break;
                    }
                    return;
                }).AsTask();
            };

            hrm.BandHeartRateReceived += (a, p) =>
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (!isWorn) return;
                    beatButton.Content = p.ToString();
                    beatButton.Foreground = new SolidColorBrush(Colors.Red);
                    await Task.Delay(200);
                    beatButton.Foreground = new SolidColorBrush(Colors.White);
                    stateChanged(p);
                }).AsTask();
            };
        }


        void stateChanged(int hb)
        {

            int flag = 0;
            if (hb < low_point) { flag = 1; }
            if (low_point <= hb && hb < high_point) { flag = 2; }
            if (high_point > 78) { flag = 3; }

            if (flag == state) return;
            else state = flag;
            playNext(state);


        }

        private async void playNext(int s)
        {

            String music = "";
            String musicName = "";
            switch (s)
            {
                case 1:
                    music = music1;
                    musicName = musicName1;
                    break;
                case 2:
                    music = music2;
                    musicName = musicName2;
                    break;
                case 3:
                    music = music3;
                    musicName = musicName3;
                    break;
            }
            try
            {
                await TTS(alertAdjustMusic + musicName);
                connectStatusInfo.Text = alertAdjustMusic;

                musicPlayer.Source = new Uri(music);
                musicPlayer.Play();
                MusicInfo.Text = musicName;

                connectStatusInfo.Text = "连接正常";

            }
            catch (Exception ex) { }
        }

        private async Task TTS(String content)
        {

            MediaElement mediaElement = this.media;
            // The object for controlling the speech synthesis engine (voice).
            SpeechSynthesizer synth = new SpeechSynthesizer();

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(content);

            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);

            mediaElement.Play();
        }


    }


}
