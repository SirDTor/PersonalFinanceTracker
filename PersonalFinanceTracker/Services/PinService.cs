using Microsoft.Maui.Storage;

namespace PersonalFinanceTracker.Services
{
    public static class PinService
    {
        private const string PinKey = "UserPin";

        public static bool IsPinSet => Preferences.ContainsKey(PinKey);

        public static void SetPin(string pin)
        {
            Preferences.Set(PinKey, pin);
        }

        public static string? GetPin()
        {
            return Preferences.Get(PinKey, null);
        }

        public static bool ValidatePin(string input)
        {
            var saved = GetPin();
            return saved != null && saved == input;
        }

        public static void RemovePin()
        {
            Preferences.Remove(PinKey);
        }
    }
}
