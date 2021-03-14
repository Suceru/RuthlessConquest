using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Engine;

namespace Game
{
    internal static class NamesManager
    {
        public static string GenerateName(int seed)
        {
            Random.Reset(seed);
            string text = null;
            int num = 0;
            while (num < 100 || text == null || ValidatePlayerName(text) != null)
            {
                text = "";
                if (Random.Bool(0.5f))
                {
                    text += CapitalizeFirst(Prefixes[Random.Int(Prefixes.Length)]);
                }
                if (Random.Bool(0.5f))
                {
                    text += CapitalizeFirst(Nouns[Random.Int(Nouns.Length)]);
                    text += CapitalizeFirst(Nouns[Random.Int(Nouns.Length)]);
                }
                else
                {
                    text += CapitalizeFirst(Adjectives[Random.Int(Adjectives.Length)]);
                    text += CapitalizeFirst(Nouns[Random.Int(Nouns.Length)]);
                }
                num++;
            }
            return text;
        }

        public static string ValidatePlayerName(string name)
        {
            if (name == "KAALUS")
            {
                return null;
            }
            if (name == null || name.Length < 4)
            {
                return "Name too short, must have at least 4 characters.";
            }
            if (name.Length > 16)
            {
                return "Name too long, must have at most 16 characters.";
            }
            if (name.Count((char c) => char.IsLetterOrDigit(c) || c == ' ') != name.Length)
            {
                return "Name can contain letters, numbers and spaces only.";
            }
            if (name.Count((char c) => char.IsUpper(c)) > 3)
            {
                return "Too many capital letters.";
            }
            string nameLower = name.ToLower();
            string nameLowerNoSpaces = nameLower.Replace(" ", "");
            if (name.StartsWith(" ") || name.EndsWith(" "))
            {
                return "Name is invalid, please try another one.";
            }
            if (Forbidden.Contains(nameLower))
            {
                return "Name is invalid, please try another one.";
            }
            if (Forbidden.Contains(nameLowerNoSpaces))
            {
                return "Name is invalid, please try another one.";
            }
            if (ForbiddenAnywhere.Any((string f) => nameLower.Contains(f)))
            {
                return "Name is invalid, please try another one.";
            }
            if (ForbiddenAnywhere.Any((string f) => nameLowerNoSpaces.Contains(f)))
            {
                return "Name is invalid, please try another one.";
            }
            return null;
        }

        public static void ShowSetPlayerNameDialog(ContainerWidget parentWidget, Action handler)
        {
            string text = string.IsNullOrEmpty(SettingsManager.PlayerName) ? GenerateName(new Engine.Random().Int()) : SettingsManager.PlayerName;
            DialogsManager.ShowDialog(parentWidget, new TextBoxDialog("Set your nickname", text, 16, delegate (string s)
            {
                if (s != null)
                {
                    s = s.Trim();
                    if (s == string.Empty)
                    {
                        SettingsManager.PlayerName = GenerateName(MathUtils.Hash(Time.FrameIndex));
                        return;
                    }
                    IEnumerable<byte> source = Encoding.UTF8.GetBytes(s).Concat(new byte[]
                    {
                        11,
                        22,
                        33,
                        44,
                        55,
                        66,
                        77,
                        88,
                        99
                    });
                    if (SHA256.Create().ComputeHash(source.ToArray<byte>()).SequenceEqual(new byte[]
                    {
                        139,
                        23,
                        161,
                        189,
                        209,
                        58,
                        22,
                        134,
                        255,
                        22,
                        179,
                        41,
                        74,
                        236,
                        236,
                        50,
                        56,
                        7,
                        55,
                        253,
                        203,
                        44,
                        150,
                        202,
                        108,
                        186,
                        7,
                        57,
                        71,
                        191,
                        73,
                        147
                    }))
                    {
                        s = "Kaalus";
                    }
                    else if (s.ToLower().Replace(" ", "").Contains("kaalus"))
                    {
                        s = "FalseKaalus";
                    }
                    string text2 = ValidatePlayerName(s);
                    if (text2 != null)
                    {
                        DialogsManager.ShowDialog(parentWidget, new MessageDialog("INVALID NAME", text2, "OK", null, null), true);
                        return;
                    }
                    SettingsManager.PlayerName = s;
                    Action handler2 = handler;
                    if (handler2 == null)
                    {
                        return;
                    }
                    handler2();
                }
            }), true);
        }

        public static void EnsureValidPlayerNameExists(Action handler)
        {
            if (ValidatePlayerName(SettingsManager.PlayerName) != null)
            {
                ShowSetPlayerNameDialog(null, handler);
                return;
            }
            if (handler != null)
            {
                handler();
            }
        }

        private static string CapitalizeFirst(string s)
        {
            if (s.Length <= 0)
            {
                return string.Empty;
            }
            return char.ToUpperInvariant(s[0]).ToString() + s.Substring(1);
        }

        private static Engine.Random Random = new Engine.Random(0);

        private static string[] Prefixes = Names.Prefixes.Replace("\r", "").Split(new char[]
{
            '\n'
});

        private static string[] Adjectives = Names.Adjectives.Replace("\r", "").Split(new char[]
{
            '\n'
});

        private static string[] Nouns = Names.Nouns.Replace("\r", "").Split(new char[]
{
            '\n'
});

        private static string[] Forbidden = Names.Forbidden.Replace("\r", "").Split(new char[]
{
            '\n'
});

        private static string[] ForbiddenAnywhere = Names.ForbiddenAnywhere.Replace("\r", "").Split(new char[]
{
            '\n'
});
    }
}
