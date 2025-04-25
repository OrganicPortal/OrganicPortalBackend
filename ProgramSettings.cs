namespace OrganicPortalBackend
{
    public static class ProgramSettings
    {
        public const int TokenExpiredMinuts = 1440;

        public const int MaxCodeCount = 10;
        public const int CodeLength = 8;

        public const string PasswordPattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
        public const string CodePattern = @"^[0-9]{8}$";
    }
}
