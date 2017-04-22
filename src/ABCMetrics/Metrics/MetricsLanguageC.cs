using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCMetrics
{
    public class MetricsLanguageC : IMetricsLanguage
    {
        private KeyTokensSet TokenSet;

        protected MetricsLanguageC()
        {
            TokenSet = new KeyTokensSetCPlus();
        }

        public MetricsLanguageC(KeyTokensSet TokenSet)
        {
            this.TokenSet = TokenSet;
        }

        /// <summary>
        /// Заменить все подстроки, которые начинаются и заканчиваются определенными подстроками
        /// </summary>
        /// <param name="CodeText">Код, в котором нужно заменить подстроки</param>
        /// <param name="StartBorder">Подстрока, с которой начинается заменяемая строка</param>
        /// <param name="EndBorder">Подстрока, которой заканчивается заменяемая строка</param>
        /// <param name="NewString">Строка, на которую нужно заменить</param>
        /// <returns>Строка с замененными подстроками</returns>
        protected string ReplaceStringWithBorders(string CodeText, string StartBorder, string EndBorder, string NewString)
        {
            while (GetStringWithBorders(CodeText, StartBorder, EndBorder) != "")
            {
                string Comment = GetStringWithBorders(CodeText, StartBorder, EndBorder);

                CodeText = CodeText.Replace(Comment, NewString);
            }

            return CodeText;
        }

        protected bool CheckFirstSymbolInLine(string CodeLine, char Symbol)
        {
            for (int i = 0; i < CodeLine.Length; i++)
            {
                if (!Char.IsWhiteSpace(CodeLine[i]))
                {
                    if (CodeLine[i] == Symbol)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Убрать все комментарии в коде
        /// </summary>
        /// <param name="CodeText">Код, в котором нужно убрать комментарии</param>
        /// <returns>Код без комментариев</returns>
        protected string ReplaceComments(string CodeText)
        {
            string NewCodeText = "";
            foreach (string lineCode in CodeText.Split('\n'))
            {
                string NewLineCode = lineCode;
                if (CheckFirstSymbolInLine(lineCode, '*'))
                {
                    continue;
                }

                int IndexMultiComment = lineCode.IndexOf("/*");
                if (IndexMultiComment != -1)
                {
                    NewLineCode = lineCode.Substring(0, IndexMultiComment);
                }

                int IndexEndMultiComment = lineCode.IndexOf("*/");
                if (IndexEndMultiComment != -1)
                {
                    NewLineCode += lineCode.Substring(IndexEndMultiComment, lineCode.Length - IndexEndMultiComment - 1);
                }

                int IndexSingleComment = NewLineCode.IndexOf("//");
                if (IndexSingleComment != -1)
                {
                    NewLineCode = lineCode.Substring(0, IndexSingleComment);
                }


                NewCodeText += NewLineCode;
            }

            return NewCodeText;
        }

        /// <summary>
        /// Посчитать АBC метрики в коде
        /// </summary>
        /// <param name="CodeText">Код, для которого нужно посчитать метрики</param>
        /// <returns>Числовое значение ABC метрики</returns>
        public double CalculateMetrics(string CodeText)
        {
            // Убрать комментарии из кода
            CodeText = ReplaceComments(CodeText);

            // Заменить все токены
            CodeText = NormalizeCodeText(CodeText, TokenSet);

            List<Lexeme> StackLexeme = new List<Lexeme>();
            Lexeme CurrentLexeme = new Lexeme();

            for (int i = 0; i < CodeText.Length; i++)
            {
                char symbol = CodeText[i];

                AnalyzeSymbol(symbol, ref CurrentLexeme, ref StackLexeme);
            }

            int A = CalculateCountTokens(StackLexeme, TypeLexeme.Assignment);
            int B = CalculateCountTokens(StackLexeme, TypeLexeme.Branch);
            int C = CalculateCountTokens(StackLexeme, TypeLexeme.Condition);

            double Result = Math.Sqrt(A * A + B * B + C * C);
            return Result;
        }

        /// <summary>
        /// Посчитать количество токенов данного типа в списке лексем
        /// </summary>
        /// <param name="Lexemes">Список лексем </param>
        /// <param name="Type">Тип лексемы, который нужно найти</param>
        /// <returns>Число лексем данного типа</returns>
        protected int CalculateCountTokens(IList<Lexeme> Lexemes, TypeLexeme Type)
        {
            int Count = 0;

            foreach (Lexeme lexeme in Lexemes)
            {
                if (lexeme.Type == Type)
                {
                    Count++;
                }
            }

            return Count;
        }

        /// <summary>
        /// Поменять все токены на ключевые слова
        /// </summary>
        /// <param name="CodeText">Исходный код</param>
        /// <param name="TokensSet">Набор токенов</param>
        /// <returns>Код с замененными токенами</returns>
        protected string NormalizeCodeText(string CodeText , KeyTokensSet TokensSet)
        {
            CodeText = ReplaceToken(CodeText, TokensSet.Branch, " BR#T ");
            CodeText = ReplaceToken(CodeText, TokensSet.AdditionToken, " AD#T ");
            CodeText = ReplaceToken(CodeText, TokensSet.Condition, " CD#T ");
            CodeText = ReplaceToken(CodeText, TokensSet.Assignment, " AG#T ");
                     
            return CodeText;
        }

        /// <summary>
        /// Функция, показывающая есть ли в строке буквы или цифры
        /// </summary>
        /// <param name="Line">Проверяемая строка</param>
        /// <returns>true если в строке есть буквы или цифры</returns>
        protected bool ContainCharactersInLine(string Line)
        {
            foreach(char symbol in Line)
            {
                if (Char.IsLetterOrDigit(symbol))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Найти строку с заданными границами 
        /// </summary>
        /// <param name="CodeText">Строка с текстом, в котором производится поиск</param>
        /// <param name="startBorder">Начальная подстрока</param>
        /// <param name="endBorder">Конечная подстрока</param>
        /// <returns>Строка, начинающаяся подстрокой startBorder и заканчивающаяся подстрокой endBorder</returns>
        protected string GetStringWithBorders(string CodeText, string startBorder, string endBorder)
        {
            string LineCode = "";
            int indexStart = CodeText.IndexOf(startBorder);
            if(indexStart>-1)
            { 
                LineCode = CodeText.Substring(indexStart);
                int indexEndFor = LineCode.IndexOf(endBorder);
                LineCode = LineCode.Substring(0, indexEndFor + 1);
            }

            return LineCode;
        }

        /// <summary>
        /// Получить символ, стоящий перед токеном
        /// </summary>
        /// <param name="CodeText">Текст с кодом</param>
        /// <param name="Token">Токен</param>
        /// <returns>Символ, стоящий перед токеном</returns>
        protected char GetPreSymbol(string CodeText, string Token)
        {
            char Symbol = ' ';
            int IndexSymbol = CodeText.IndexOf(Token);

            if (IndexSymbol > 0)
                Symbol = CodeText.ElementAt(IndexSymbol-1);

            return Symbol;
        }

        /// <summary>
        /// Найти индекс первого вхождения токена в строку
        /// </summary>
        /// <param name="CodeText">Исходная строка, в которой производится поиск</param>
        /// <param name="Token">Токен, который нужно найти</param>
        /// <returns>Индекс первого вхождения токена. Если токен не найден, то -1</returns>
        protected int ContainsToken(string CodeText, string Token)
        {
            int IndexToken = CodeText.IndexOf(Token);
            string NewCodeText = "";
            int NewIndexToken;

            if (IndexToken > -1)
            {
                int LengthToken = Token.Length;

                if (IndexToken > 0)
                {
                    char PreSymbol = CodeText.ElementAt(IndexToken - 1);
                    // Если перед токеном стоит буква или цифра
                    if (Char.IsLetterOrDigit(PreSymbol))
                    {
                        NewCodeText = CodeText.Substring(IndexToken+ LengthToken, CodeText.Length- IndexToken - LengthToken);
                        NewIndexToken = ContainsToken(NewCodeText, Token);

                        if(NewIndexToken>-1)
                        {
                            return NewIndexToken + LengthToken + IndexToken;
                        }

                        return -1;
                    }
                }
                // Токен стоит в "середине" текста
                if (IndexToken + LengthToken < CodeText.Length)
                {
                    char PostSymbol = CodeText.ElementAt(IndexToken + LengthToken);

                    if (Char.IsLetterOrDigit(PostSymbol) == false)
                    {
                        return IndexToken;
                    }
                }
                // Токен стоит в конце текста
                else
                {
                    return IndexToken;
                }

                NewCodeText = CodeText.Substring(IndexToken + LengthToken, CodeText.Length - IndexToken - LengthToken);
                NewIndexToken = ContainsToken(NewCodeText, Token);

                if (NewIndexToken > -1)
                {
                    return NewIndexToken + LengthToken + IndexToken;
                }

                return -1;
            }
            else
            {
                return -1;
            }

        }

        /// <summary>
        /// Заменить слово в тексте с указанного индекса
        /// </summary>
        /// <param name="CodeText">Текст</param>
        /// <param name="Word">Заменяемое слово</param>
        /// <param name="NewWord">Новое слово</param>
        /// <param name="StartIndex">Индекс символа, с которого нужно производить замену</param>
        /// <returns>Строка с замененными словами</returns>
        protected string ReplaceWordBeginIndex(string CodeText, string Word, string NewWord, int StartIndex)
        {
            if (StartIndex != -1)
            {
                string LineWithWord = CodeText.Substring(StartIndex);
                string LineBefore = CodeText.Substring(0, StartIndex);
                LineWithWord = ReplaceFirstWord(LineWithWord, Word, NewWord);
                return LineBefore + LineWithWord;
            }

            return CodeText;
        }

        /// <summary>
        /// Заменить первое вхождение подстроки в строке
        /// </summary>
        /// <param name="Text">Строка, в которой производится замена</param>
        /// <param name="Word">Подстрока, которую нужно заменить</param>
        /// <param name="NewWord">Подстрока, на которую нужно заменить</param>
        /// <returns>Строка, с замененной подстрокой</returns>
        protected string ReplaceFirstWord(string Text, string Word, string NewWord)
        {
            int IndexFirst = Text.IndexOf(Word);

            if( IndexFirst>-1)
                Text = Text.Remove(IndexFirst, Word.Length).Insert(IndexFirst, NewWord);

            return Text;
        }

        /// <summary>
        /// Раскрыть цикл for на условие и действие
        /// </summary>
        /// <param name="CodeText">Исходный код</param>
        /// <returns>Код, с раскрытым циклом for</returns>
        protected string ReplaceTokenFor(string CodeText)
        {
            int IndexForInCode = ContainsToken(CodeText, "for");
            string CodeTextCopy = CodeText;
            while (IndexForInCode != -1)
            {
                string NewLineCodeFor = "";
                string LineWithToken = CodeTextCopy.Substring(IndexForInCode);
                string LineCodeFor = GetStringWithBorders(LineWithToken, "for", ")");
                int IndexFor = -1;

                if (LineCodeFor.Split(';').Count() > 1)
                {
                    // Получить условие выхода из цикла for
                    string LineConditionCicle = LineCodeFor.Split(';').ElementAt(1);

                    // Если это условие не пустое, то добавить токен условия и ветки                                      
                    if (ContainCharactersInLine(LineConditionCicle))
                    {
                        NewLineCodeFor = LineCodeFor.Replace(LineConditionCicle, " CD#T " + LineConditionCicle);
                        IndexFor = ContainsToken(NewLineCodeFor, "for");
                        if (IndexFor != -1)
                        {
                            NewLineCodeFor = ReplaceWordBeginIndex(NewLineCodeFor, "for", " BR#T ", IndexFor);
                        }
                    }
                    else
                    {
                        IndexFor = ContainsToken(LineCodeFor, "for");
                        if (IndexFor != -1)
                        {
                            NewLineCodeFor = ReplaceWordBeginIndex(LineCodeFor, "for", " BR#T ", IndexFor);
                        }
                    }
                }
                else
                {
                    IndexFor = ContainsToken(LineCodeFor, "for");
                    if (IndexFor != -1)
                    {
                        NewLineCodeFor = ReplaceWordBeginIndex(LineCodeFor, "for", " BR#T ", IndexFor);
                    }
                }

                if (IndexFor != -1)
                {
                    // Поменять в коде строку с for(), на строку с добавленными токенами
                    CodeText = CodeText.Replace(LineCodeFor, NewLineCodeFor);
                }
                CodeTextCopy = CodeTextCopy.Substring(IndexForInCode + 1);
                IndexForInCode = ContainsToken(CodeTextCopy, "for");

            }
            return CodeText;
        }

        /// <summary>
        /// Раскрыть цикл while на условия, при их наличии
        /// </summary>
        /// <param name="CodeText">Исходный код</param>
        /// <returns>Код, с раскрытым циклом while</returns>
        protected string ReplaceTokenWhile(string CodeText)
        {
            string CopyCodeText = CodeText;
            while (ContainsToken(CopyCodeText, "while") != -1)
            {
                int IndexWhile = ContainsToken(CopyCodeText, "while");
                string LineWithToken = CopyCodeText.Substring(IndexWhile);
                string LineCodeWhileCondition = GetStringWithBorders(LineWithToken, "(", ")");
                if (ContainCharactersInLine(LineCodeWhileCondition))
                {

                    string LineCodeWhile = GetStringWithBorders(LineWithToken, "while", ")");
                    IndexWhile = ContainsToken(LineCodeWhile, "while");

                    string NewLineCodeFor = ReplaceWordBeginIndex(LineCodeWhile, "while", " CD#T ", IndexWhile);

                    CodeText = CodeText.Replace(LineCodeWhile, NewLineCodeFor);
                    CopyCodeText = CodeText;
                }
                else
                {
                    CopyCodeText = CodeText.Substring(IndexWhile + "while".Length);
                }
            }
            return CodeText;
        }

        /// <summary>
        /// Заменить в коде токены из колекции на новый токен
        /// </summary>
        /// <param name="CodeText">Код, в котором производится замена</param>
        /// <param name="CollectionToken">Коллекция токенов</param>
        /// <param name="NewToken">Новый токен</param>
        /// <returns>Код с замененными токенами</returns>
        protected string ReplaceToken(string CodeText, IReadOnlyCollection<string> CollectionToken, string NewToken)
        {
            foreach (string token in CollectionToken)
            {
                if (NewToken == " AG#T " || NewToken == " AD#T ")
                {
                    CodeText = CodeText.Replace(token, NewToken);
                }
                else
                {
                    int IndexTokenInCode = ContainsToken(CodeText, token);
                    // Если токен найден в тексте       
                    while (IndexTokenInCode != -1)
                    {
                        int IndexToken = ContainsToken(CodeText, token);

                        if (token == "for")
                        {
                            CodeText = ReplaceTokenFor(CodeText);
                            break;
                        }
                        else if (token == "while")
                        {
                            CodeText = ReplaceTokenWhile(CodeText);
                            break;
                        }
                        else if (token == "?")
                        {
                            NewToken = " CD#T  CD#T ";
                            CodeText = CodeText.Replace(token, NewToken);
                            NewToken = " CD#T ";
                        }
                        else
                        {
                            CodeText = ReplaceWordBeginIndex(CodeText, token, NewToken, IndexToken);
                        }

                        IndexTokenInCode = ContainsToken(CodeText.Substring(IndexTokenInCode + 1), token);

                        
                    }
                }
            }

            return CodeText;
        }

        /// <summary>
        /// Анализировать символ для построения лексемы
        /// </summary>
        /// <param name="symbol">Анализируемый символ</param>
        /// <param name="CurrentLexeme">Текущая исследуемая лексема</param>
        /// <param name="StackLexeme">Стэк уже найденных лексем</param>
        protected void AnalyzeSymbol(char symbol, ref Lexeme CurrentLexeme, ref List<Lexeme> StackLexeme)
        {
            if (Char.IsWhiteSpace(symbol) || symbol == ',' || symbol == ';' || symbol == ')'
                || symbol == '+' || symbol == '-' || symbol == '*' || symbol == '/'
                || symbol == '%')
            {

                if (ContainCharactersInLine(CurrentLexeme.Name))
                {
                    if (CurrentLexeme.Name.Contains("AG#T"))
                        CurrentLexeme.Type = TypeLexeme.Assignment;
                    else if (CurrentLexeme.Name.Contains("BR#T"))
                        CurrentLexeme.Type = TypeLexeme.Branch;
                    else if (CurrentLexeme.Name.Contains("CD#T"))
                        CurrentLexeme.Type = TypeLexeme.Condition;
                    else if (CurrentLexeme.Name.Contains("AD#T"))
                        CurrentLexeme.Type = TypeLexeme.Addition;

                    if (CurrentLexeme.Name != " ")
                    {
                        StackLexeme.Add(CurrentLexeme);
                    }
                    CurrentLexeme = new Lexeme();
                }
                else
                    CurrentLexeme.Name += symbol;

            }
            else if (symbol == '(')
            {
                if (ContainCharactersInLine(CurrentLexeme.Name))
                {
                    if (StackLexeme.Count() > 0)
                    {
                        if (StackLexeme.Last().Type == TypeLexeme.Addition ||
                            StackLexeme.Last().Type == TypeLexeme.Condition ||
                            StackLexeme.Last().Type == TypeLexeme.Assignment)
                        {
                            CurrentLexeme.Type = TypeLexeme.Branch;
                        }
                    }
                    else
                    {
                        CurrentLexeme.Type = TypeLexeme.Branch;
                    }
                    StackLexeme.Add(CurrentLexeme);
                }

                CurrentLexeme = new Lexeme();
            }
            else
            {
                CurrentLexeme.Name += symbol;
            }
        }
    }
}

