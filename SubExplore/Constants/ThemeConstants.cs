using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Constants
{
    public static class ThemeConstants
    {
        // Couleurs du design system
        public static class Colors
        {
            // Couleurs principales
            public const string Primary = "#006994";     // Bleu océan profond
            public const string Secondary = "#00B4D8";   // Bleu lagon
            public const string Accent = "#48CAE4";      // Bleu clair
            public const string Warning = "#FF9F1C";     // Orange corail
            public const string Success = "#2EC4B6";     // Turquoise
            public const string Background = "#F8FDFF";  // Blanc azuré

            // Variantes de couleurs principales
            public const string PrimaryLight = "#007AB0";
            public const string PrimaryDark = "#005577";
            public const string SecondaryLight = "#1FC3E7";
            public const string SecondaryDark = "#0095B9";

            // Couleurs de texte
            public const string TextPrimary = "#1A1A1A";
            public const string TextSecondary = "#666666";
            public const string TextDisabled = "#999999";
            public const string TextOnPrimary = "#FFFFFF";

            // Couleurs de feedback
            public const string Error = "#DC2626";
            public const string ErrorLight = "#FEE2E2";
            public const string Info = "#3B82F6";
            public const string InfoLight = "#DBEAFE";

            // Couleurs de statut
            public const string Active = "#10B981";
            public const string Inactive = "#6B7280";
            public const string Pending = "#F59E0B";
        }

        // Mesures standards
        public static class Sizes
        {
            // Espacements
            public const int DefaultPadding = 16;
            public const int SmallPadding = 8;
            public const int LargePadding = 24;
            public const int ExtraLargePadding = 32;

            // Rayons de bordure
            public const int BorderRadius = 8;
            public const int SmallBorderRadius = 4;
            public const int LargeBorderRadius = 12;
            public const int CircularBorderRadius = 9999;

            // Tailles de texte
            public const int HeadingLarge = 24;
            public const int HeadingMedium = 20;
            public const int HeadingSmall = 16;
            public const int BodyText = 14;
            public const int SmallText = 12;

            // Élévations (shadows)
            public const int ElevationSmall = 2;
            public const int ElevationMedium = 4;
            public const int ElevationLarge = 8;

            // Tailles d'icônes
            public const int IconSmall = 16;
            public const int IconMedium = 24;
            public const int IconLarge = 32;

            // Hauteurs de composants
            public const int ButtonHeight = 48;
            public const int InputHeight = 56;
            public const int CardMinHeight = 100;
        }

        // Animation
        public static class Animation
        {
            public const int ShortDuration = 150;
            public const int MediumDuration = 250;
            public const int LongDuration = 350;

            public const string DefaultEasing = "cubic-bezier(0.4, 0, 0.2, 1)";
            public const string AccelerateEasing = "cubic-bezier(0.4, 0, 1, 1)";
            public const string DecelerateEasing = "cubic-bezier(0, 0, 0.2, 1)";
        }

        // Z-Index
        public static class ZIndex
        {
            public const int Modal = 1000;
            public const int Overlay = 900;
            public const int Drawer = 800;
            public const int AppBar = 700;
            public const int FloatingButton = 600;
            public const int Content = 1;
        }

        // Breakpoints
        public static class Breakpoints
        {
            public const int Mobile = 640;
            public const int Tablet = 768;
            public const int Desktop = 1024;
            public const int DesktopLarge = 1280;
        }
    }
}
