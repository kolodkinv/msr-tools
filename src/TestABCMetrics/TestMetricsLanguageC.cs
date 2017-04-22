using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCMetrics;


namespace TestABCMetrics
{
    [TestFixture]
    public class TestMetricsLanguageC : MetricsLanguageC
    {
        [Test]
        public void CalculateMetricsTest()
        {
            // A= 0; B=1; C=0;
            string TestLine = @"int main()
                                {                    
                                    cout << helloworld;
                                    cin.get();
                                }";
            double Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(1, Result);

            // A= 0; B=1; C=0;
            TestLine = @"get();";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(1, Result);

            // A= 0; B=0; C=0;
            TestLine = @" ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(0, Result);

            // A= 1; B=1; C=0;
            TestLine = @"int main()
                         {    
                            int helloworld = 1;               
                            cout << helloworld;
                            cin.get();
                         }";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(1.414, Result);

            // A= 1; B=1; C=1;
            TestLine = @"int main()
                         {    
                            int helloworld = 1;               
                            cout << helloworld;
                            if(helloworld>0)
                            cin.get();
                         }";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(1.732, Result);

            // A= 2; B=2; C=2;
            TestLine = @"int main()
                         {   
                            int a = 10; 
                            int helloworld = func(a);   
                            // prinf(helloworld);\n       
                            cout << helloworld;
                            if(a>0 && helloworld >0)
                            cin.get();
                         }";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(3.464, Result);

            // A= 2; B=4; C=2;
            TestLine = @"bool SearchList(char *cmd, char **list, unsigned n) 
                        {
                            // search entire list for command string
                            for(unsigned i=0; i < n; i++)
                                if (stricmp(cmd, list[i]) == 0) return true;
                                    return false;
                        } ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(4.899, Result);

            // A= 2; B=4; C=2;
            TestLine = @"bool SearchList(char *cmd, char **list, unsigned n) 
                        {
                            /* search entire list for command string
                            comment */
                            for(unsigned i=0; i < n; i++)
                                if (stricmp(cmd, list[i]) == 0) return true;
                                    return false;
                        } ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(4.899, Result);

            // A= 6; B=9; C=3;
            TestLine = @"static ushort CalculatePageSize(void)
                        {
                            const ushort MAX_NVRAM_PAGE_SIZE = 256;
                            utiny save;
                            ushort i, n;
                            outportb(0x800,1);
                            for(i=1; i < MAX_NVRAM_PAGE_SIZE; i*=2)
                            {
                                n = i + 5;
                                save = inportb(0x800+n);
                                outNVRAM(0x800+n,0x55);
                                if (inportb(0x800+n) != 0x55)
                                {
                                    outNVRAM(0x800+n,save);
                                    break;
                                }
                                outNVRAM(0x800+n,save);
                            }
                            if (i == 1)
                                i = 0;
                            return i;
                        } ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(11.225, Result);

            // A= 4; B=14; C=1;
            TestLine = @"static ushort UartStream::Transmit(unsigned char *data)
                        {
                            unsigned char i(0), n = data[1] + 3;
                            data[n-1] = Checksum(n-1, data);
                            memcpy(itsLastMessage, data, n);
                            WaitReceiveFifoEmpty();
                            disable();
                            SetParityEven();
                            SendByte(data[0]);
                            SendByte(data[1]);
                            SendByte(data[2]);
                            WaitTransmitRegisterEmpty();
                            SetParityOdd();
                            enable();
                            for(i=3; i < n; i++)
                                SendByte(data[i]);
                            delay(itsXmtDelay);
                        }  ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(14.595, Result);

            // A= 7; B=23; C=14;
            TestLine = @"void UpdateSchedule(ADI_EVENT *evt)
                        {
                            EVENT tempevt;
                            char am=0;
                            if (!Lock(evt->file, evt->data.rec)
                            {
                                DisplayMessage(errorMessage[2], 0x71);
                                return;
                            }
                            // Comment
                            ReadLog(&tempevt, evt->file, evt->data.rec);
                            if (memcp(&tempevt, &evt->data, sizeof(tempevt)))
                            {
                                if (memcmp(tempevt.src, evt->data.src, sizeof(tempevt.src)) == 0 &&
                                    memcmp(tempevt.house, evt->data.house, sizeof(tempevt.house)) == 0)
                                {
                                    memcpy(&evt->data, &tempevt, sizeof(evt->data));
                                }
                                else
                                {
                                    UnLock(evt->file, evt->data.rec);
                                    DisplayMessage(errorMessage[5], 0x71);
                                    updateFlag = 1;
                                }
                            }
                            /* Comment
                            Comment*/
                            evt->data.adistate[0] = evt->adiStatus++;
                            switch(evt->adiStatus)
                            {
                                case 0x11:
                                    memcpy(evt->data.src, device.utility, sizeof(evt->data.src));
                                    break;
                                case 0x23:
                                    if (device.mode == 19 || device.channel == 2)
                                        memcpy(evt->data.src, device.utility, sizeof(evt->data.src));
                                    break;
                                default:
                                    if (device.mode == 1)
                                    {
                                         memcpy(evt->data.src, device.utility, sizeof(evt->data.src));
                                         evt->data.adistate[0] = 0;
                                    }
                                    break;
                             }
                            am = evt->data.amarc[0];
                            evt->data.amarc[0] = (am >= 0 && am <= 0xF) ? ‘’ : ‘C’;
                        } ";
            Result = Math.Round(CalculateMetrics(TestLine), 3);
            Assert.AreEqual(27.821, Result);
        }

        [Test]
        public void GetPreSymbolTest()
        {
            string TestLine = "for(int i=1; i<10;i++)";
            char Result = GetPreSymbol(TestLine, "for");
            Assert.AreEqual(' ', Result);

            TestLine = " for(int i=1; i<10;i++)";
            Result = GetPreSymbol(TestLine, "for");
            Assert.AreEqual(' ', Result);

            TestLine = "afor(int i=1; i<10;i++)";
            Result = GetPreSymbol(TestLine, "for");
            Assert.AreEqual('a', Result);
        }

        [Test]
        public void ReplaceWordBeginIndexTest()
        {
            string TestLine = "for(int i=1; i<10;i++)";
            string Result = ReplaceWordBeginIndex(TestLine, "for", "FOR", 0);
            Assert.AreEqual("FOR(int i=1; i<10;i++)", Result);

            TestLine = "for(int i=1; i<10;i++)";
            Result = ReplaceWordBeginIndex(TestLine, "for", "FOR", 2);
            Assert.AreEqual("for(int i=1; i<10;i++)", Result);

            TestLine = "ufor(int i=1; i<10;i++)";
            Result = ReplaceWordBeginIndex(TestLine, "for", "FOR", 0);
            Assert.AreEqual("uFOR(int i=1; i<10;i++)", Result);
        }

        [Test]
        public void ReplaceFirstWordTest()
        {
            string TestLine = "for(int i=1; i<10;i++)";
            string Result = ReplaceFirstWord(TestLine, "for", "FOR");
            Assert.AreEqual("FOR(int i=1; i<10;i++)", Result);

            TestLine = "fo(int i=1; i<10;i++)";
            Result = ReplaceFirstWord(TestLine, "for", "FOR");
            Assert.AreEqual("fo(int i=1; i<10;i++)", Result);

            TestLine = "for(int ufor=1; i<10;i++)";
            Result = ReplaceFirstWord(TestLine, "for", "FOR");
            Assert.AreEqual("FOR(int ufor=1; i<10;i++)", Result);

            TestLine = "or(int ufor=1; i<10;i++)";
            Result = ReplaceFirstWord(TestLine, "for", "FOR");
            Assert.AreEqual("or(int uFOR=1; i<10;i++)", Result);
        }

        [Test]
        public void ContainsTokenTest()
        {
            string TestLine = "for(int i=1; i<10;i++)";
            int Result = ContainsToken(TestLine, "for");
            Assert.AreEqual(0, Result);

            TestLine = "int a=1;for(int i=1; i<10;i++)";
            Result = ContainsToken(TestLine, "for");
            Assert.AreEqual(8, Result);

            TestLine = "int a=1;fora(int i=1; i<10;i++)";
            Result = ContainsToken(TestLine, "for");
            Assert.AreEqual(-1, Result);

            TestLine = "ufor(int i=1; i<10;i++)";
            Result = ContainsToken(TestLine, "for");
            Assert.AreEqual(-1, Result);

            TestLine = " ufor(); for(int i=1; i<10 && a<10;i++)";
            Result = ContainsToken(TestLine, "for");
            Assert.AreEqual(9, Result);
        }

        [Test]
        public void ReplaceTokenWhileTest()
        {
            string TestLine = "while(true)";
            string Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual(" CD#T (true)", Result);

            TestLine = "for(int i=1; func(i)<10;i++)";
            Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual("for(int i=1; func(i)<10;i++)", Result);

            TestLine = "while()";
            Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual("while()", Result);

            TestLine = "awhile()";
            Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual("awhile()", Result);

            TestLine = "awhile(); while()";
            Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual("awhile(); while()", Result);

            TestLine = "awhile(); while(true)";
            Result = ReplaceTokenWhile(TestLine);
            Assert.AreEqual("awhile();  CD#T (true)", Result);
        }



        [Test]
        public void ReplaceTokenForTest()
        {
            string TestLine = "for(int i=1; i<10;i++)";
            string Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T (int i=1; CD#T  i<10;i++)", Result);

            TestLine = "for(int i=1; func(i)<10;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T (int i=1; CD#T  func(i)<10;i++)", Result);

            TestLine = "for(int i=1; i<10 && a<10;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T (int i=1; CD#T  i<10 && a<10;i++)", Result);

            TestLine = "for(int i=1; ;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T (int i=1; ;i++)", Result);

            TestLine = ";for(int i=1; ;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual("; BR#T (int i=1; ;i++)", Result);

            TestLine = "for     (int i=1; ;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T      (int i=1; ;i++)", Result);

            TestLine = "for(;;)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" BR#T (;;)", Result);

            TestLine = " ufor()";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" ufor()", Result);

            TestLine = " foru()";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" foru()", Result);

            TestLine = " ufor(); for(int i=1; i<10 && a<10;i++)";
            Result = ReplaceTokenFor(TestLine);
            Assert.AreEqual(" ufor();  BR#T (int i=1; CD#T  i<10 && a<10;i++)", Result);
        }

        [Test]
        public void GetStringWithBordersTest()
        {
            string TestLine = "int a = 1;";
            string Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("", Result);

            TestLine = "for(kek=1;";
            Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("", Result);

            TestLine = "func(1); for(kek=1;";
            Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("", Result);

            TestLine = "for(kek=1;k<2;k++)";
            Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("for(kek=1;k<2;k++)", Result);

            TestLine = "for(kek=1;k<2;k++)";
            Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("for(kek=1;k<2;k++)", Result);

            TestLine = "func(1); for(kek=1;k<2;k++) { func(a);} ";
            Result = GetStringWithBorders(TestLine, "for", ")");
            Assert.AreEqual("for(kek=1;k<2;k++)", Result);
        }


        [Test]
        public void ContainCharactersInLineTest()
        {
            string TestLine = "";
            bool Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(false, Result);

            TestLine = " ";
            Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(false, Result);

            TestLine = ",(#";
            Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(false, Result);

            TestLine = ",1(#";
            Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(true, Result);

            TestLine = ",k(#";
            Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(true, Result);

            TestLine = "test";
            Result = ContainCharactersInLine(TestLine);
            Assert.AreEqual(true, Result);
        }
    }
}
