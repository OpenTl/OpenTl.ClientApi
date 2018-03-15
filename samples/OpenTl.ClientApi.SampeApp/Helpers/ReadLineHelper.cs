namespace OpenTl.ClientApi.SampeApp.Helpers
{
    using System;

    public static class ReadLineHelper
    {
        public static string Read(string promt)
        {
            var line = ReadInternal(promt);

            ReadLine.AddHistory(line);

            return line;
        }

        private static string ReadInternal(string promt)
        {
            string line;
            do
            {
                line = ReadLine.Read(promt);
            }
            while (string.IsNullOrWhiteSpace(line));

            return line;
        }

        public static string ReadPassword(string promt)
        {
            ReadLine.PasswordMode = true;

            var line = ReadInternal(promt);

            ReadLine.PasswordMode = false;

            return line;
        }
    }
}