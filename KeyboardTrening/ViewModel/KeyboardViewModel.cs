using KeyboardTrening.Model;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace KeyboardTrening.ViewModel
{
    class KeyboardViewModel : INotifyPropertyChanged
    {   
        MainWindow _window; //Передаем во вьюмодель ссылку на окно, для поиска по контенту окна
        ISettings _settings; //настройки приложения раскладки и тп.
        DispatcherTimer timer = null;
        int seconds = 0;
        string randStrFromfile = "";
        string strFromFile = "";
        bool IsLoadFile = false;
        public KeyboardViewModel(Window window)
        {
            _window = window as MainWindow;
            _settings = new KeyboardTrainerSettings();
            //обновляем клавиатуру (в сеттингах устанавливается системный язык, и включен ли капслок)
            if (!_settings.FlagCapslock)
            {
                LoverLetters();
                LoverSymbol();
            }
            else
            {
                CapitalLetters();
                LoverSymbol();
            }
            timer = new DispatcherTimer();
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        }

        //Свойсва Кнопок View
        //start stop идет ли игра
        private bool _isStartBtn = true;
        public bool IsStartBtn
        {
            get { return _isStartBtn; }
            set
            {
                _isStartBtn = value;
                OnPropertyChanged("IsStartBtn");
            }
        }
        private bool _isStopBtn = false;
        public bool IsStopBtn
        {
            get { return _isStopBtn; }
            set
            {
                _isStopBtn = value;
                OnPropertyChanged("IsStopBtn");
            }
        }
        private bool _isLoadBtn = true;
        public bool IsLoadBtn
        {
            get { return _isLoadBtn; }
            set
            {
                _isLoadBtn = value;
                OnPropertyChanged("IsLoadBtn");
            }
        }
        private bool _isClearBtn = false;
        public bool IsClearBtn
        {
            get { return _isClearBtn; }
            set
            {
                _isClearBtn = value;
                OnPropertyChanged("IsClearBtn");
            }
        }

        //Свойства Текстблоков
        private string _speedCount = "0";
        public string SpeedCount
        {
            get { return _speedCount; }
            set
            {
                _speedCount = value;
                OnPropertyChanged("SpeedCount");
            }
        }
        private string _failsCount = "0";
        public string FailsCount
        {
            get { return _failsCount; }
            set
            {
                _failsCount = value;
                OnPropertyChanged("FailsCount");
            }
        }
        private string _difficulty = "5";
        public string Difficulty
        {
            get { return _difficulty; }
            set
            {
                _difficulty = value;
                OnPropertyChanged("Difficulty");
            }
        }
        private string _strLen = "70";
        public string StrLen
        {
            get { return _strLen; }
            set
            {
                _strLen = value;
                OnPropertyChanged("StrLen");
            }
        }
        //Свойство чекбокса
        private bool _isCaseSensitive = false;
        public bool IsCaseSensitive
        {
            get { return _isCaseSensitive; }
            set
            {
                _isCaseSensitive = value;
                OnPropertyChanged("IsCaseSensitive");
            }
        }
        private bool _chBoxSensIsEnabled = true; //добавлено
        public bool ChBoxSensIsEnabled
        {
            get { return _chBoxSensIsEnabled; }
            set
            {
                _chBoxSensIsEnabled = value;
                OnPropertyChanged("ChBoxSensIsEnabled");
            }
        }
        //Свойства текстбоксов
        //programmTextBox
        private string _programmTBText;
        public string ProgrammTBText
        {
            get { return _programmTBText; }
            set
            {
                _programmTBText = value;
                OnPropertyChanged("ProgrammTBText");
            }
        }
        //userTextBox
        private string _userTBText;
        public string UserTBText
        {
            get { return _userTBText; }
            set
            {
                _userTBText = value;
                OnPropertyChanged("UserTBText");
            }
        }
        private bool _userTBIsReadOnly = true; //добавлено
        public bool UserTBIsReadOnly 
        {
            get { return _userTBIsReadOnly; }
            set
            {
                _userTBIsReadOnly = value;
                OnPropertyChanged("UserTBIsReadOnly");
            }
        }
        private bool _userTBIsEnabled = false; //добавлено
        public bool UserTBIsEnabled
        {
            get { return _userTBIsEnabled; }
            set
            {
                _userTBIsEnabled = value;
                OnPropertyChanged("UserTBIsEnabled");
            }
        }
        //Свойсвта слайдера  
        private bool _sliderIsEnabled = true; //добавлено
        public bool SliderIsEnabled
        {
            get { return _sliderIsEnabled; }
            set
            {
                _sliderIsEnabled = value;
                OnPropertyChanged("SliderIsEnabled");
            }
        }

        //Команды клавиатуры
        //Команда для нажатие клавиш
        KeyCommand<KeyEventArgs> _previewKDCommand;
        public KeyCommand<KeyEventArgs> PreviewKDCommand => _previewKDCommand ?? (_previewKDCommand = new KeyCommand<KeyEventArgs>(KeyDown));
        //Команда для отпускания клавиш
        KeyCommand<KeyEventArgs> _previewKUCommand;
        public KeyCommand<KeyEventArgs> PreviewKUCommand => _previewKUCommand ?? (_previewKUCommand = new KeyCommand<KeyEventArgs>(KeyUp));

        //Команды для кнопок
        //Start
        ActionCommand _clickStartCommand;
        public ActionCommand ClickStartCommand => _clickStartCommand ?? (_clickStartCommand = new ActionCommand(BtnStartClick));
        //Stop
        ActionCommand _clickStopCommand;
        public ActionCommand ClickStopCommand => _clickStopCommand ?? (_clickStopCommand = new ActionCommand(BtnStopClick));
        //LoadFile
        ActionCommand _clickLoadCommand;
        public ActionCommand ClickLoadCommand => _clickLoadCommand ?? (_clickLoadCommand = new ActionCommand(BtnLoadClick));
        //Clear
        ActionCommand _clickClearCommand;
        public ActionCommand ClickClearCommand => _clickClearCommand ?? (_clickClearCommand = new ActionCommand(BtnClearClick));
        //Команда слайдера
        ActionCommand _sliderCommand;
        public ActionCommand SliderCommand => _sliderCommand ?? (_sliderCommand = new ActionCommand(SliderValueChanged));
        ActionCommand _strLenSliderCommand;
        public ActionCommand StrLenSliderCommand => _strLenSliderCommand ?? (_strLenSliderCommand = new ActionCommand(StrLenSliderValueChanged));
        //Команды ТекстБоксов
        ActionCommand _textBoxCommandChanged;
        public ActionCommand TextBoxCommandChanged => _textBoxCommandChanged ?? (_textBoxCommandChanged = new ActionCommand(TextChanged));
        //Команда ЧекБокса
        ActionCommand _checkCommand;
        public ActionCommand CheckCommand => _checkCommand ?? (_checkCommand = new ActionCommand(CheckedUnchecked));

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        //Методы(действия) для комманд
        //действие на нажатие кнопки
        private void KeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == (ModifierKeys.Shift | ModifierKeys.Alt))
            {
                if (!_settings.FlagLanguage)
                {
                    _settings.FlagLanguage = true;
                    LoverLetters();
                    LoverSymbol();
                }
                else
                {
                    _settings.FlagLanguage = false;
                    LoverLetters();
                    LoverSymbol();
                }
                return;
            }


            foreach (UIElement element in (_window.Content as Grid).Children)
            {         
                if (element is Grid)
                {
                    foreach (var item in (element as Grid).Children)
                    {
                        if (item is Button)
                        {
                            if ((item as Button).Name == e.Key.ToString())
                            {
                                (item as Button).Opacity = 0.5;
                                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                                {
                                    CapitalSymbol();
                                    if (!_settings.FlagCapslock)
                                    {
                                        CapitalLetters();
                                    }
                                    else
                                    {
                                        LoverLetters();
                                    }
                                }
                                else if (e.Key == Key.CapsLock)
                                {
                                    if (!_settings.FlagCapslock)
                                    {
                                        _settings.FlagCapslock = true;
                                        CapitalLetters();
                                    }
                                    else
                                    {
                                        _settings.FlagCapslock = false;
                                        LoverLetters();                                       
                                    }
                                }
                                else if (e.Key == Key.Back)
                                {
                                    _settings.FlagBackspace = true;
                                }
                                else { _settings.FlagBackspace = false; }
                            }
                        }
                    }
                }
            }
        }
        //действие на отпускание кнопки
        private void KeyUp(KeyEventArgs e)
        {
            _window.userTextBox.Focus();
            foreach (UIElement element in (_window.Content as Grid).Children)
            {
                if (element is Grid)
                {
                    foreach (var item in (element as Grid).Children)
                    {
                        if (item is Button)
                        {
                            if ((item as Button).Name == e.Key.ToString())
                            {
                                (item as Button).Opacity = 1;
                                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                                {
                                    LoverSymbol();
                                    if (!_settings.FlagCapslock)
                                    {
                                        LoverLetters();
                                    }
                                    else
                                    {
                                        CapitalLetters();
                                    }
                                }                             
                            }
                        }
                    }
                }
            }
            if (_settings.FlagStopGame)
            {
                MessageBox.Show($"Задание завершенно!\n Длина строки: {ProgrammTBText.Length}.\n Количество ошибок: {FailsCount}.\nВаша текущая скорость: {SpeedCount} сим/мин.", "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetSettings();
            }
        }
        //Кнопки
        private void BtnStartClick(object parametr)
        {          
            IsStartBtn = false;
            IsStopBtn = true;
            
            //1.Включение-выключение элементов управления и обоих TB
            SliderIsEnabled = false;
            ChBoxSensIsEnabled = false;
            UserTBIsReadOnly = false;
            UserTBIsEnabled = true;
            //2.Генерируется строка в programmTB с учетом сложности
            if (!IsLoadFile)
            {
                AsseblyString(_settings.GetKeyboardLayout(), Convert.ToInt32(Difficulty), IsCaseSensitive); //если не загружали файл то просто генерируем рандомную строку
            }
            else
            {
                randStrFromfile = GenerateRandomStringFromLoadText(strFromFile);
                ProgrammTBText = randStrFromfile;
            }
            _window.userTextBox.Focus();
            UserTBText = "";
            //3.Стартуем таймер
            seconds = 0;
            timer.Start();
        }
        private void BtnStopClick(object parametr)
        {
            _settings.FlagStopGame = true;
            ResetSettings();
        }
        private void BtnLoadClick(object parametr) 
        {          
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "Text Files|*.txt";
                if (fileDialog.ShowDialog() == true)
                {
                    strFromFile = File.ReadAllText(fileDialog.FileName);
                }
                //randStrFromfile = GenerateRandomStringFromLoadText(strFromFile);
                IsLoadFile = true;
                IsLoadBtn = false;
                IsClearBtn = true;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка чтения файла: " + Ex.Message);
            }
        }
        private void BtnClearClick(object parametr) 
        {
            IsLoadFile = false;
            randStrFromfile = "";
            IsLoadBtn = true;
            IsClearBtn = false;
        }

        //слайдер
        private void SliderValueChanged(object parametr)
        {
            if (parametr is Slider)
            {
                Slider temp = parametr as Slider;
                Difficulty = ((int)temp.Value).ToString();
            }
        }
        private void StrLenSliderValueChanged(object parametr)
        {
            if (parametr is Slider)
            {
                Slider temp = parametr as Slider;
                StrLen = ((int)temp.Value).ToString();
            }
        }
        private void CheckedUnchecked(object parametr)
        {
            (parametr as Slider).Maximum = IsCaseSensitive ? _settings.BaseStringEn.Length : 47;
        }

        //текстбокс
        private void TextChanged(object parametr)
        {
            if (!_settings.FlagStopGame) //если не конец игры
            {
                (parametr as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                if (UserTBText.Length != 0) //если TB не пустой (допустим юзер ввел и стер первый символ)
                {
                    try
                    {
                        string str = ProgrammTBText.Substring(0, UserTBText.Length);
                        char uLastChar = UserTBText.Last();
                        char pLastChar = str.Last();

                        if (uLastChar.Equals(pLastChar))
                        {
                            if (_settings.FlagBackspace)
                            {
                                SpeedGame();
                            }
                            (parametr as TextBox).Foreground = UserTBText.Equals(str) ? Brushes.Green : Brushes.Red;
                        }
                        else
                        {
                            _settings.Fails++;
                            (parametr as TextBox).Foreground = Brushes.Red;
                            FailsCount = _settings.Fails.ToString();
                        }
                        if (UserTBText.Length == ProgrammTBText.Length) //остановка игры
                        {
                            _settings.FlagStopGame = true;
                            timer.Stop();
                            SpeedGame();
                        }
                    }
                    catch (Exception Ex)
                    {
                        MessageBox.Show(Ex.Message);
                    }
                }             
            }      
        }

        //методы замены символов
        private void CapitalSymbol()
        {
            if (!_settings.FlagLanguage)
            {
                _window.Oem3.Content = "~";
                _window.D1.Content = "!";
                _window.D2.Content = "@";
                _window.D3.Content = "#";
                _window.D4.Content = "$";
                _window.D5.Content = "%";
                _window.D6.Content = "^";
                _window.D7.Content = "&";
                _window.D8.Content = "*";
                _window.D9.Content = "(";
                _window.D0.Content = ")";
                _window.OemMinus.Content = "_";
                _window.OemPlus.Content = "+";
                _window.OemOpenBrackets.Content = "{";
                _window.Oem6.Content = "}";
                _window.Oem5.Content = "|";
                _window.Oem1.Content = ":";
                _window.OemQuotes.Content = "\"";
                _window.OemComma.Content = "<";
                _window.OemPeriod.Content = ">";
                _window.OemQuestion.Content = "?";
            }
            else
            {              
                _window.D1.Content = "!";
                _window.D2.Content = "\"";
                _window.D3.Content = "№";
                _window.D4.Content = ";";
                _window.D5.Content = "%";
                _window.D6.Content = ":";
                _window.D7.Content = "?";
                _window.D8.Content = "*";
                _window.D9.Content = "(";
                _window.D0.Content = ")";
                _window.OemMinus.Content = "_";
                _window.OemPlus.Content = "+";               
                _window.Oem5.Content = "/";               
                _window.OemQuestion.Content = ",";

                
            }
        }
        private void LoverSymbol()
        {
            if (!_settings.FlagLanguage)
            {
                _window.Oem3.Content = "`";
                _window.D1.Content = "1";
                _window.D2.Content = "2";
                _window.D3.Content = "3";
                _window.D4.Content = "4";
                _window.D5.Content = "5";
                _window.D6.Content = "6";
                _window.D7.Content = "7";
                _window.D8.Content = "8";
                _window.D9.Content = "9";
                _window.D0.Content = "0";
                _window.OemMinus.Content = "-";
                _window.OemPlus.Content = "=";
                _window.OemOpenBrackets.Content = "[";
                _window.Oem6.Content = "]";
                _window.Oem5.Content = "\\";
                _window.Oem1.Content = ";";
                _window.OemQuotes.Content = "'";
                _window.OemComma.Content = ",";
                _window.OemPeriod.Content = ".";
                _window.OemQuestion.Content = "/";
            }
            else
            {               
                _window.D1.Content = "1";
                _window.D2.Content = "2";
                _window.D3.Content = "3";
                _window.D4.Content = "4";
                _window.D5.Content = "5";
                _window.D6.Content = "6";
                _window.D7.Content = "7";
                _window.D8.Content = "8";
                _window.D9.Content = "9";
                _window.D0.Content = "0";
                _window.OemMinus.Content = "-";
                _window.OemPlus.Content = "=";            
                _window.Oem5.Content = "\\";
                _window.OemQuestion.Content = ".";
            }
        }
        private void CapitalLetters()
        {
            if (!_settings.FlagLanguage)
            {
                _window.Q.Content = "Q";
                _window.W.Content = "W";
                _window.E.Content = "E";
                _window.R.Content = "R";
                _window.T.Content = "T";
                _window.Y.Content = "Y";
                _window.U.Content = "U";
                _window.I.Content = "I";
                _window.O.Content = "O";
                _window.P.Content = "P";
                _window.A.Content = "A";
                _window.S.Content = "S";
                _window.D.Content = "D";
                _window.F.Content = "F";
                _window.G.Content = "G";
                _window.H.Content = "H";
                _window.J.Content = "J";
                _window.K.Content = "K";
                _window.L.Content = "L";
                _window.Z.Content = "Z";
                _window.X.Content = "X";
                _window.C.Content = "C";
                _window.V.Content = "V";
                _window.B.Content = "B";
                _window.N.Content = "N";
                _window.M.Content = "M";
            }
            else
            {
                _window.Q.Content = "Й";
                _window.W.Content = "Ц";
                _window.E.Content = "У";
                _window.R.Content = "К";
                _window.T.Content = "Е";
                _window.Y.Content = "Н";
                _window.U.Content = "Г";
                _window.I.Content = "Ш";
                _window.O.Content = "Щ";
                _window.P.Content = "З";
                _window.A.Content = "Ф";
                _window.S.Content = "Ы";
                _window.D.Content = "В";
                _window.F.Content = "А";
                _window.G.Content = "П";
                _window.H.Content = "Р";
                _window.J.Content = "О";
                _window.K.Content = "Л";
                _window.L.Content = "Д";
                _window.Z.Content = "Я";
                _window.X.Content = "Ч";
                _window.C.Content = "С";
                _window.V.Content = "М";
                _window.B.Content = "И";
                _window.N.Content = "Т";
                _window.M.Content = "Ь";
                _window.Oem3.Content = "Ё";
                _window.Oem1.Content = "Ж";
                _window.OemQuotes.Content = "Э";
                _window.OemComma.Content = "Б";
                _window.OemPeriod.Content = "Ю";
                _window.OemOpenBrackets.Content = "Х";
                _window.Oem6.Content = "Ъ";
            }
        }
        private void LoverLetters()
        {
            if (!_settings.FlagLanguage)
            {
                _window.Q.Content = "q";
                _window.W.Content = "w";
                _window.E.Content = "e";
                _window.R.Content = "r";
                _window.T.Content = "t";
                _window.Y.Content = "y";
                _window.U.Content = "u";
                _window.I.Content = "i";
                _window.O.Content = "o";
                _window.P.Content = "p";
                _window.A.Content = "a";
                _window.S.Content = "s";
                _window.D.Content = "d";
                _window.F.Content = "f";
                _window.G.Content = "g";
                _window.H.Content = "h";
                _window.J.Content = "j";
                _window.K.Content = "k";
                _window.L.Content = "l";
                _window.Z.Content = "z";
                _window.X.Content = "x";
                _window.C.Content = "c";
                _window.V.Content = "v";
                _window.B.Content = "b";
                _window.N.Content = "n";
                _window.M.Content = "m";
            }
            else
            {
                _window.Q.Content = "й";
                _window.W.Content = "ц";
                _window.E.Content = "у";
                _window.R.Content = "к";
                _window.T.Content = "е";
                _window.Y.Content = "н";
                _window.U.Content = "г";
                _window.I.Content = "ш";
                _window.O.Content = "щ";
                _window.P.Content = "з";
                _window.A.Content = "ф";
                _window.S.Content = "ы";
                _window.D.Content = "в";
                _window.F.Content = "а";
                _window.G.Content = "п";
                _window.H.Content = "р";
                _window.J.Content = "о";
                _window.K.Content = "л";
                _window.L.Content = "д";
                _window.Z.Content = "я";
                _window.X.Content = "ч";
                _window.C.Content = "с";
                _window.V.Content = "м";
                _window.B.Content = "и";
                _window.N.Content = "т";
                _window.M.Content = "ь";
                _window.OemOpenBrackets.Content = "х";
                _window.Oem6.Content = "ъ";
                _window.Oem1.Content = "ж";
                _window.OemQuotes.Content = "э";
                _window.OemComma.Content = "б";
                _window.OemPeriod.Content = "ю";
                _window.Oem3.Content = "ё";
            }
        }

        //Вспомогательные методы
        private void AsseblyString(string baseString, int countChars,  bool isCaseSensitive) //Сборка строки с учетом регистра сложности и раскладки языка
        {
            Random rnd = new Random();
            StringBuilder builderChars = new StringBuilder();
            builderChars.Append(" ");
            int stringLength = Convert.ToInt32(StrLen); //длина рандомной строки, берем из слайдера (сколько указал пользователь)
            int charPosition = isCaseSensitive ? 0 : 47; //ru или eng раскладка (с учетом регистра) позиция в базовой строке откуда будут выбираться символы
            for (int i = 0; i < countChars; i++)
            {
                builderChars.Append(baseString[rnd.Next(charPosition, baseString.Length)]);
            }
            StringBuilder builderStr = new StringBuilder();
            for (int i = 0; i < stringLength; i++)
            {
                var ch = builderChars[rnd.Next(0, builderChars.Length)];
                if ((i == 0 || i == stringLength-1) && ch == ' ') //первый и последний символ - не даем встать пробелу
                {
                    i--;
                    continue;
                }
                builderStr.Append(ch);
            }
            ProgrammTBText = builderStr.ToString();
        }
        private void TimerTick(object sender, EventArgs e) //тик таймера
        {
            seconds++;
            SpeedGame();
        }
        private void SpeedGame() //обновление счетчика скорости
        {
            try
            {
                SpeedCount = Math.Round(((double)UserTBText.Length / seconds) * 60).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ResetSettings() //сброс настроек
        {
            IsStartBtn = true;
            IsStopBtn = false;
            SliderIsEnabled = true;
            ChBoxSensIsEnabled = true;
            UserTBText = "";
            ProgrammTBText = "";
            UserTBIsReadOnly = true;
            UserTBIsEnabled = false;
            seconds = 0;
            SpeedCount = "0";
            FailsCount = "0";           
            _settings.Fails = 0;
            _settings.FlagStopGame = false;
        }

        private string GenerateRandomStringFromLoadText(string str) //генерация рандомной строки из загруженного текста
        {
            str.Replace("\n", " ");
            StringBuilder result = new StringBuilder();
            int length = Convert.ToInt32(StrLen);
            try
            {
                Random rnd = new Random();
                int startIndex;        
                do
                {
                    startIndex = rnd.Next(0, str.Length - Convert.ToInt32(StrLen) - 1);
                } while (str[startIndex] != (' ' | '.')); //ищем индекс пока не наткнемся на пробел или точку
                startIndex++;

                for (int i = startIndex; i < startIndex + length; i++)
                {
                    if (str[i] != '\n')
                    {
                        result.Append(str[i]);
                    }                 
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка Генерации строки: " + Ex.Message);
            }
            return result.ToString() ?? "null";
        }
    }
}
