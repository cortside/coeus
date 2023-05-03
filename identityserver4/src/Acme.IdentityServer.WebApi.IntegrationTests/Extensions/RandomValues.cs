using System;
using System.Text;

namespace Acme.IdentityServer.WebApi.IntegrationTests.Extensions {
    public static class RandomValues {
        private static readonly object syncLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="random"></param>
        /// <param name="lengthStart"></param>
        /// <param name="lengthEnd"></param>
        /// <param name="useSpecialChars"></param>
        /// <returns></returns>
        public static string RandomString(this Random random, int lengthStart = 5, int lengthEnd = 20, bool useSpecialChars = false) {
            lock (syncLock) {
                int strLength = 0;
                // Decide how long the string will be
                strLength = random.Next(lengthStart, lengthEnd);
                var sb = new StringBuilder();
                for (int i = 0; i < strLength; i++) {
                    // Decide whether to add an uppercase letter, a lowercase letter, a number, or a special character
                    int whichType;
                    if (useSpecialChars) {
                        whichType = random.Next(0, 4);
                    } else {
                        whichType = random.Next(0, 3);
                    }
                    switch (whichType) {
                        // Lower case letter
                        case 0:
                            sb.Append((char)(97 + random.Next(0, 26)));
                            break;
                        // Upper case letter
                        case 1:
                            sb.Append((char)(65 + random.Next(0, 26)));
                            break;
                        // Number
                        case 2:
                            sb.Append((char)(48 + random.Next(0, 10)));
                            break;
                        // Special character
                        case 3:
                            int whichSet;
                            whichSet = random.Next(0, 4);
                            switch (whichSet) {
                                case 0:
                                    sb.Append((char)(32 + random.Next(0, 16)));
                                    break;
                                case 1:
                                    sb.Append((char)(58 + random.Next(0, 7)));
                                    break;
                                case 2:
                                    sb.Append((char)(91 + random.Next(0, 6)));
                                    break;
                                case 3:
                                    sb.Append((char)(123 + random.Next(0, 4)));
                                    break;
                            }
                            break;
                    }
                }
                return sb.ToString();
            }
        }
    }
}
