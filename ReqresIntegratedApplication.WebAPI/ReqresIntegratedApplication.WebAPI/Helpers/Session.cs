namespace ReqresIntegratedApplication.WebAPI.Helpers
{
    /// <summary>
    /// In-memory token store for the TeamShift Lite session.
    /// </summary>
    public static class Session
    {
        public static string? CurrentToken { get; set; }

        public static bool IsAuthenticated => !string.IsNullOrWhiteSpace(CurrentToken);

        public static void Clear()
        {
            CurrentToken = null;
        }
    }
}
