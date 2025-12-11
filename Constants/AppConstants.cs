namespace PlaywrightTests.Constants
{
    public static class AppConstants
    {
        public const string BaseUrl = "https://www.spinberry.com/#products";


        public static class Selectors
        {
            public const string VideoSlider = ".row.video-slider";
            public const string GameCard = ".col-4.center";
            public const string GameTitle = "h4";
            public const string GameName = "h4";
            public const string PlayButton = "a.showreel.play";
            public const string ProgressBarHidden = ".attract__progress-bar[data-hidden='false']";

            // Slider Controls
            public const string SliderControlsWrapper = ".slider__controls-wrapper";
            public const string SliderLeftButton = ".button__slider-left";
            public const string SliderPlayButton = ".button__slider-play";
            public const string SliderRightButton = ".button__slider-right";
            public const string MessageDialog = ".error__description";
            public const string CongratulationsText = "CONGRATULATIONS!";
            public const string WonText = "YOU HAVE WON";
        }

        public static class Texts
        {
            public const string CongratulationsText = "CONGRATULATIONS!";
            public const string WonText = "YOU HAVE WON";

        }
    }
}

