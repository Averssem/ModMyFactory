﻿using System.Net;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using ModMyFactory.Helpers;

namespace ModMyFactory.Web
{
    static class ApiAuthentication
    {
        /// <summary>
        /// Logs in at the api.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The users password.</param>
        /// <param name="token">Out. The login token generated by the server.</param>
        /// <returns>Returns false if the login failed, otherwise true.</returns>
        public static bool LogIn(string username, SecureString password, out string token)
        {
            const string loginPage = "https://auth.factorio.com/api-login";
            const string pattern = "[0-9a-f]{30}";

            token = null;

            string part1 = $"require_game_ownership=True&username={username}&password=";
            int part1Length = Encoding.UTF8.GetByteCount(part1);
            int part2Length = SecureStringHelper.GetSecureStringByteCount(password);

            byte[] content = new byte[part1Length + part2Length];
            Encoding.UTF8.GetBytes(part1, 0, part1.Length, content, 0);
            SecureStringHelper.SecureStringToBytes(password, content, part1Length);

            try
            {
                string document = WebHelper.GetDocument(loginPage, null, content);
                MatchCollection matches = Regex.Matches(document, pattern,
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                if (matches.Count != 1) return false;

                token = matches[0].Value;
                return true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError) return false;
                else throw;
            }
            finally
            {
                SecureStringHelper.DestroySecureByteArray(content);
            }
        }
    }
}
