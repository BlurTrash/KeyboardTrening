using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace KeyboardTrening.Model
{
   
    internal class KeyboardTrainerSettings : ISettings
    {
        public string BaseStringEn { get; } = "QWERTYUIOPASDFGHJKLZXCVBNM~!@#$%^&*()_+{}|:\"<>?1234567890[],./\\`-=;'qwertyuiopasdfghjklzxcvbnm";
        public string BaseStringRu { get; } = "ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ!\"№;%:?*()_+/,.\\1234567890-=йцукенгшщзхъфывапролджэячсмитьбюё";
        public int Fails { get; set; } = 0;
        public bool FlagCapslock { get; set; }
        public bool FlagBackspace { get; set; } = false;
        public bool FlagStopGame { get; set; } = false;
        public bool FlagLanguage { get; set; }  // false - Eng раскладка, true - Ru раскладка
        public KeyboardTrainerSettings()
        {
            FlagLanguage = (InputLanguageManager.Current.CurrentInputLanguage.ToString() == "en-US") ? false : true;
            if (Keyboard.GetKeyStates(Key.CapsLock) == KeyStates.Toggled)
            {
                FlagCapslock = true;
            }
            else
                FlagCapslock = false;
        }
        public string GetKeyboardLayout() //получить текущую расскладку клавиатуры
        {
            return FlagLanguage ? BaseStringRu : BaseStringEn;
        }
    }

   
}
