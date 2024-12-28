using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubExplore.Constants
{
    public static class ApiEndpoints
    {
        // URL de base de l'API
        public const string BaseUrl = "https://api.subexplore.com/";
        public const string ApiVersion = "v1";

        // Routes de l'API
        public static class Auth
        {
            private const string Base = "auth";
            public const string Login = $"{Base}/login";
            public const string Register = $"{Base}/register";
            public const string ForgotPassword = $"{Base}/forgot-password";
            public const string ResetPassword = $"{Base}/reset-password";
            public const string RefreshToken = $"{Base}/refresh-token";
            public const string ValidateEmail = $"{Base}/validate-email";
            public const string ChangePassword = $"{Base}/change-password";
            public const string CurrentUser = $"{Base}/me";
        }

        public static class Spots
        {
            private const string Base = "spots";
            public const string GetAll = Base;
            public const string Create = Base;
            public const string GetById = $"{Base}/{{0}}";
            public const string Update = $"{Base}/{{0}}";
            public const string Delete = $"{Base}/{{0}}";
            public const string GetNearby = $"{Base}/nearby";
            public const string UploadMedia = $"{Base}/{{0}}/media";
            public const string DeleteMedia = $"{Base}/{{0}}/media/{{1}}";
            public const string Validate = $"{Base}/{{0}}/validate";
            public const string Report = $"{Base}/{{0}}/report";
            public const string Rate = $"{Base}/{{0}}/rate";
            public const string GetComments = $"{Base}/{{0}}/comments";
            public const string AddComment = $"{Base}/{{0}}/comments";
        }

        public static class Users
        {
            private const string Base = "users";
            public const string GetProfile = $"{Base}/{{0}}";
            public const string UpdateProfile = $"{Base}/{{0}}";
            public const string GetSpots = $"{Base}/{{0}}/spots";
            public const string GetRatings = $"{Base}/{{0}}/ratings";
            public const string UploadAvatar = $"{Base}/{{0}}/avatar";
            public const string UpdateSettings = $"{Base}/{{0}}/settings";
        }

        public static class Organizations
        {
            private const string Base = "organizations";
            public const string GetAll = Base;
            public const string Create = Base;
            public const string GetById = $"{Base}/{{0}}";
            public const string Update = $"{Base}/{{0}}";
            public const string Delete = $"{Base}/{{0}}";
            public const string Validate = $"{Base}/{{0}}/validate";
            public const string GetMembers = $"{Base}/{{0}}/members";
            public const string AddMember = $"{Base}/{{0}}/members";
            public const string UpdateMember = $"{Base}/{{0}}/members/{{1}}";
        }

        public static class Media
        {
            private const string Base = "media";
            public const string Upload = Base;
            public const string GetById = $"{Base}/{{0}}";
            public const string Delete = $"{Base}/{{0}}";
            public const string GetVariants = $"{Base}/{{0}}/variants";
            public const string Optimize = $"{Base}/{{0}}/optimize";
        }

        public static class Moderation
        {
            private const string Base = "moderation";
            public const string GetPending = $"{Base}/pending";
            public const string GetReports = $"{Base}/reports";
            public const string HandleReport = $"{Base}/reports/{{0}}";
            public const string GetStats = $"{Base}/stats";
        }
    }
}
