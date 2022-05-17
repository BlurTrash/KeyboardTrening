using System;
using System.Collections.Generic;
using System.Text;

namespace KeyboardTrening.Model
{
    interface ISettings
    {
        string BaseStringEn { get; } //ангилийская раскладка
        string BaseStringRu { get; } //проименовал неудачно, сдесь может быть любая другая расскладка
        int Fails { get; set; }
        bool FlagCapslock { get; set; }
        bool FlagBackspace { get; set; }
        bool FlagStopGame { get; set; }
        bool FlagLanguage { get; set; }
        string GetKeyboardLayout();
    }
}
