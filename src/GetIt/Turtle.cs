using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace GetIt
{
    public static class Turtle
    {
        private static readonly string AssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static readonly Player DefaultPlayer = Player.Create(new Size(50, 50), DefaultCostume);

        private static PlayerOnScene @default;
        public static PlayerOnScene Default
        {
            get
            {
                return @default ?? throw new Exception($"Default player hasn't been added to the scene. Consider calling `{nameof(Game)}.{nameof(Game.ShowSceneAndAddTurtle)}` at the beginning.");
            }

            internal set
            {
                if (@default != null)
                {
                    throw new Exception("Default player has already been set.");
                }
                @default = value;
            }
        }

        private static Costume DefaultCostume =>
            new Costume(
                new Size(500, 500),
                ImmutableList<GeometryPath>.Empty
                    .Add(new GeometryPath(
                        fill: new RGBA(0x59, 0x75, 0x28, 0xff),
                        data: "M 41.852373,281.09037 H 29.542649 c -14.129167,0 -23.2419233,-8.17863 -27.2528093,-18.63842 C 0.76644172,258.49895 -8.5927788e-4,254.2318 7.2211636e-7,250.00617 -8.5927788e-4,245.78957 0.77162372,241.5134 2.2889787,237.55407 6.2955527,227.10782 15.406579,218.93641 29.540918,218.94543 l 12.311465,-0.0216 C 48.176415,176.2279 67.308187,137.95084 95.002918,108.55743 L 79.64295,80.73385 C 67.422987,58.62807 71.187026,37.416894 82.591341,22.217826 86.911226,16.455779 92.406678,11.551303 98.565848,7.8339127 104.69742,4.1165167 111.5117,1.5861997 118.48393,0.54806872 136.76122,-2.1691113 156.18732,5.0156307 168.38829,27.114171 L 183.75431,54.917 c 15.49548,-4.203965 31.74877,-6.449026 48.5044,-6.448125 16.75389,-9.01e-4 33.02186,2.245062 48.5182,6.448125 L 296.1084,27.135837 c 12.19407,-22.1184053 31.64002,-29.3112663 49.91731,-26.58686128 6.98605,1.04353998 13.80809,3.57475798 19.93362,7.27139698 6.0953,3.6812833 11.59852,8.5992933 15.96759,14.4254363 11.38793,15.19275 15.15283,36.403021 2.95013,58.485321 l -15.36342,27.8254 c 15.43334,16.36356 28.18842,35.49578 37.56615,56.59862 1.16176,-0.16428 2.35198,-0.30782 3.54739,-0.41524 2.65149,-0.25638 5.14589,-0.38636 7.48492,-0.38456 22.60322,-0.002 43.09009,9.59228 57.90285,25.08474 14.81276,15.49245 23.98593,36.91938 23.98506,60.56066 8.7e-4,23.64129 -9.17317,45.06732 -23.98506,60.55888 -14.81189,15.49155 -35.29704,25.08562 -57.90285,25.08653 -2.46073,-8.9e-4 -5.07856,-0.14443 -7.81895,-0.41614 -1.08838,-0.10743 -2.15259,-0.23472 -3.21164,-0.38547 -9.38117,21.09563 -22.14058,40.21972 -37.56874,56.59954 l 15.3591,27.82268 c 12.22256,22.10306 8.45507,43.31786 -2.94666,58.51422 -4.31298,5.75663 -9.80756,10.66019 -15.97451,14.3839 -6.12897,3.7165 -12.94584,6.24772 -19.91807,7.28403 -18.27557,2.71719 -37.71116,-4.46664 -49.90437,-26.56428 L 280.7631,445.08271 c -15.50324,4.20307 -31.75135,6.44632 -48.50525,6.44723 -16.75304,0 -33.01323,-2.24326 -48.51649,-6.44994 l -15.33148,27.78477 c -12.19494,22.11751 -31.64175,29.31127 -49.91646,26.58596 -6.98777,-1.04534 -13.80982,-3.57657 -19.933612,-7.26958 -6.09876,-3.68311 -11.600932,-8.60219 -15.966563,-14.42834 -11.389823,-15.18896 -15.156452,-36.40284 -2.954616,-58.48423 l 15.372929,-27.8281 C 67.321132,362.03894 48.174684,323.7718 41.852373,281.09037"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0xe0, 0x9a, 0x14, 0xff),
                        data: "M 358.84122,382.39164 c 32.39267,-33.87903 52.43501,-80.6913 52.43415,-132.39268 8.6e-4,-51.7014 -20.03976,-98.51908 -52.43329,-132.39359 -32.38575,-33.87721 -77.15113,-54.83924 -126.5851,-54.83924 -49.43225,0 -94.19073,20.96203 -126.58338,54.84105 -32.384892,33.8709 -52.432438,80.68857 -52.434159,132.39088 0.0027,51.69597 20.047536,98.51635 52.432429,132.39448 32.39416,33.87383 77.16149,54.83924 126.58598,54.83834 49.43483,-9.1e-4 94.19849,-20.96835 126.58337,-54.83924"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0x77, 0x61, 0x1e, 0xff),
                        data: "M 391.48936,250.00075 c -0.002,45.97366 -17.82848,87.62327 -46.64007,117.75691 -28.83574,30.12464 -68.64515,48.7793 -112.58972,48.7793 -43.94456,0 -83.75482,-18.65378 -112.56469,-48.7784 -28.831672,-30.1614 -46.665974,-71.79771 -46.665974,-117.75871 0,-45.961 17.834512,-87.59889 46.637494,-117.73073 28.81159,-30.16071 68.63307,-48.80637 112.59231,-48.80546 43.97132,9e-4 83.78071,18.64474 112.59059,48.77658 28.81159,30.13364 46.63834,71.77062 46.64006,117.76051"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0xfb, 0xc7, 0x1b, 0xff),
                        data: "M 179.72266 154.87305 L 127.19922 250 L 179.7207 345.125 L 284.79492 345.12305 L 311.05664 297.56055 L 337.31836 250 L 284.79492 154.875 L 179.72266 154.87305 z"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0xf1, 0xae, 0x0d, 0xff),
                        data: "M 232.25586 97.761719 C 208.17585 97.762619 185.45705 103.87922 165.44922 114.71094 L 179.72852 140.57617 L 284.79492 140.57617 L 299.06836 114.71094 C 279.05966 103.88012 256.3376 97.762619 232.25586 97.761719 z M 153.65039 121.85547 C 144.85268 127.77279 136.69071 134.65122 129.33008 142.34961 C 104.49828 168.30634 88.601817 203.64651 86.865234 242.85156 L 115.42773 242.85156 L 167.92969 147.7168 L 153.65039 121.85547 z M 310.87305 121.85547 L 296.58594 147.72656 L 349.08203 242.85156 L 377.6543 242.85352 C 375.91772 203.66288 360.0171 168.32068 335.18359 142.34766 C 327.82211 134.64839 319.67596 127.77279 310.87305 121.85547 z M 86.863281 257.14844 C 88.600733 296.34719 104.49931 331.69203 129.31836 357.66211 C 136.6859 365.3623 144.84273 372.24736 153.64648 378.16016 L 167.93164 352.28125 L 115.42773 257.14844 L 86.863281 257.14844 z M 349.08203 257.14844 L 296.58594 352.27539 L 310.87695 378.16016 C 319.6833 372.24103 327.836 365.36516 335.19922 357.66406 C 360.01894 331.67662 375.91771 296.33906 377.6543 257.14844 L 349.08203 257.14844 z M 284.79492 359.42188 L 179.72852 359.42383 L 165.45117 385.29688 C 185.46678 396.11867 208.17757 402.23738 232.25586 402.23828 C 256.34709 402.23828 279.0681 396.11941 299.07422 385.29492 L 284.79492 359.42188 z"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0x87, 0xb0, 0x27, 0xff),
                        data: "M 170.3234,59.10652 156.60507,34.256483 c -8.941,-16.23539 -23.02183,-21.534349 -36.19295,-19.58087 -5.25032,0.779049 -10.39015,2.688298 -15.01818,5.483115 -4.63405,2.779475 -8.776132,6.490541 -12.072362,10.880459 C 85.11164,41.985559 82.482583,57.3751 91.429628,73.58973 L 105.17558,98.51919 C 124.00009,81.23214 146.07853,67.70761 170.3234,59.10652 Z m -65.14091,342.37399 -13.746812,24.9069 c -8.960849,16.24894 -6.314554,31.65473 1.88762,42.60199 3.25394,4.33666 7.415872,8.04142 12.070652,10.84977 4.63578,2.80295 9.77303,4.7113 14.99833,5.48402 13.18492,1.95889 27.25799,-3.35179 36.21797,-19.58177 l 13.733,-24.84732 c -24.24573,-8.60923 -46.32849,-22.12924 -65.16076,-39.41359 z m 189.01239,39.40277 13.71746,24.85995 c 8.93927,16.23358 23.01407,21.53163 36.19381,19.58177 5.25033,-0.78086 10.38844,-2.68829 15.01731,-5.48402 4.63493,-2.78037 8.77787,-6.49416 12.07237,-10.88047 8.20218,-10.93823 10.84071,-26.3395 1.89108,-42.55144 l -13.74595,-24.92946 c -18.81587,17.28164 -40.89432,30.79894 -65.14608,39.40367 z M 424.9471,249.99896 c -10e-6,25.0432 -4.36737,49.02301 -12.35203,71.14053 1.70379,0.1372 3.54136,0.20672 5.51702,0.20762 18.83746,-9e-4 35.90121,-7.99178 48.23856,-20.89523 12.33821,-12.90437 19.9785,-30.7511 19.97849,-50.45203 8.7e-4,-19.70183 -7.64114,-37.54677 -19.97935,-50.45111 -12.33821,-12.90438 -29.39938,-20.89614 -48.23684,-20.89523 -2.00414,-0.002 -3.84257,0.0686 -5.51529,0.20762 7.98121,22.12473 12.34944,46.0946 12.34944,71.13783 z M 359.33407,98.51106 373.08088,73.6114 c 8.96084,-16.24894 6.3154,-31.655631 -1.88678,-42.601095 -3.25393,-4.338464 -7.41586,-8.041419 -12.07237,-10.851584 -4.63318,-2.803841 -9.7739,-4.712183 -14.99659,-5.484007 -13.19097,-1.957997 -27.25885,3.352696 -36.21884,19.58267 L 294.17934,59.09839 c 24.25176,8.59929 46.33107,22.12111 65.15473,39.41267 z M 40.232321,233.21291 H 29.543518 c -7.64977,-0.0279 -12.516016,4.2184 -14.596121,9.64554 -0.845848,2.20806 -1.276541,4.66796 -1.27741,7.14772 8.6e-4,2.47977 0.429831,4.93968 1.279141,7.1423 2.071471,5.4127 6.937699,9.64465 14.59179,9.64465 h 10.692263 c -0.437604,-5.54179 -0.662874,-11.13954 -0.663743,-16.79327 -8.61e-4,-5.65553 0.227009,-11.25238 0.662883,-16.78694 v 0"
                    ))
                    .Add(new GeometryPath(
                        fill: new RGBA(0x1c, 0x1a, 0x19, 0xff),
                        data: "M 462.08342,284.21559 c -2.52116,-4.1191 -9.50719,-4.13355 -15.5956,-0.0497 -6.08668,4.0875 -8.97724,10.73603 -6.4535,14.84881 2.52029,4.1164 9.51324,4.13084 15.59733,0.0479 6.08235,-4.0902 8.98069,-10.73601 6.45177,-14.84699 z m 0.008,-68.43146 c -2.52978,4.1191 -9.51496,4.13985 -15.60595,0.0515 -6.08236,-4.0902 -8.97638,-10.74053 -6.45955,-14.84971 2.52807,-4.11729 9.51497,-4.13986 15.60424,-0.0497 6.09185,4.09113 8.97638,10.73874 6.46126,14.84792 v 0"
                    ))
            );

        public static void MoveTo(double x, double y) => Default.MoveTo(x, y);
        public static void MoveToCenter() => Default.MoveToCenter();
        public static void Move(double deltaX, double deltaY) => Default.Move(deltaX, deltaY);
        public static void MoveRight(double steps) => Default.MoveRight(steps);
        public static void MoveLeft(double steps) => Default.MoveLeft(steps);
        public static void MoveUp(double steps) => Default.MoveUp(steps);
        public static void MoveDown(double steps) => Default.MoveDown(steps);
        public static void MoveInDirection(double steps) => Default.MoveInDirection(steps);
        public static void MoveToRandomPosition() => Default.MoveToRandomPosition();
        public static void SetDirection(Degrees angle) => Default.SetDirection(angle);
        public static void RotateClockwise(Degrees angle) => Default.RotateClockwise(angle);
        public static void RotateCounterClockwise(Degrees angle) => Default.RotateCounterClockwise(angle);
        public static void BounceIfOnEdge() => Default.BounceIfOnEdge();
        public static void TurnUp() => Default.TurnUp();
        public static void TurnRight() => Default.TurnRight();
        public static void TurnDown() => Default.TurnDown();
        public static void TurnLeft() => Default.TurnLeft();
        public static void Say(string text) => Default.Say(text);
        public static void Say(string text, double durationInSeconds) => Default.Say(text, durationInSeconds);
        public static string Ask(string question) => Default.Ask(question);
        public static void ShutUp() => Default.ShutUp();
        public static void TurnOnPen() => Default.TurnOnPen();
        public static void TurnOffPen() => Default.TurnOffPen();
        public static void TogglePenOnOff() => Default.TogglePenOnOff();
        public static void SetPenColor(RGBA color) => Default.SetPenColor(color);
        public static void ShiftPenColor(double shift) => Default.ShiftPenColor(shift);
        public static void SetPenWeight(double weight) => Default.SetPenWeight(weight);
        public static void ChangePenWeight(double change) => Default.ChangePenWeight(change);
        public static void SetSizeFactor(double sizeFactor) => Default.SetSizeFactor(sizeFactor);
        public static void ChangeSizeFactor(double change) => Default.ChangeSizeFactor(change);
        public static Degrees GetDirectionToMouse() => Default.GetDirectionToMouse();
        public static double GetDistanceToMouse() => Default.GetDistanceToMouse();
        public static IDisposable OnKeyDown(KeyboardKey key, Action<PlayerOnScene> action) => Default.OnKeyDown(key, action);
        public static IDisposable OnMouseEnter(Action<PlayerOnScene> action) => Default.OnMouseEnter(action);
        public static IDisposable OnClick(Action<PlayerOnScene> action) => Default.OnClick(action);
    }
}