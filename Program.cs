using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace ctmRegMe
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Версия: {0}", Const.APP.version);
            Console.Write("Поиск файла {0}", Const.INFILE.reglist);
            if (File.Exists(Path.Combine(Const.APP.dir, Const.INFILE.reglist)))
            {
                ok();
                if (checkRegInfoFile())
                {
                    Console.Write("Чтение кода компьютера");
                    XmlDocument xdoc = new XmlDocument();
                    try
                    {
                        xdoc.Load(Const.SYS.fullpath);
                        string сompCode;
                        if (xdoc.SelectSingleNode("//*[local-name() = 'ComputerCode']") != null)
                        {
                            сompCode = xdoc.SelectSingleNode("//*[local-name() = 'ComputerCode']").InnerText;
                            ok();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(сompCode);
                            Console.ResetColor();
                            string activation = GetActivation(сompCode);
                            Console.Write("Поиск кода активации");
                            if (!string.IsNullOrEmpty(activation))
                            {
                                ok();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(activation);
                                Console.ResetColor();
                                if (xdoc.SelectSingleNode(String.Format("//*[text() = '{0}']", activation)) == null)
                                {
                                    XmlNode regItem = xdoc.SelectSingleNode("/Registration");
                                    if (regItem != null)
                                    {
                                        bool addRegItems = true;
                                        if (xdoc.SelectSingleNode("/Registration/RegistrationItems") != null)
                                        {
                                            addRegItems = false;
                                            regItem = xdoc.SelectSingleNode("/Registration/RegistrationItems");
                                        }


                                        using (XmlWriter writer = regItem.CreateNavigator().AppendChild())
                                        {
                                            if (addRegItems) writer.WriteStartElement("RegistrationItems");
                                            writer.WriteStartElement("RegistrationItem");
                                            writer.WriteElementString("ActivationCode", activation);
                                            writer.WriteElementString("RegistrationType", "0");
                                            writer.WriteElementString("ServerAddress", "");
                                            writer.WriteElementString("ServerPort", "");
                                            writer.WriteElementString("Inactive", "0");
                                        }

                                        Console.Write("Запись активации в файл");
                                        try
                                        {
                                            xdoc.Save(Const.SYS.fullpath);
                                            ok();
                                        }
                                        catch (Exception err)
                                        {
                                            Cancel();
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("Во время записи произошла ошибка {0}.", err.Message);
                                        }

                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Не найден раздел для активации ");
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Активация не требуется");
                                }
                            }
                            else
                            {
                                Cancel();
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Не найден подходящий код активации.");
                            }

                        }
                        else
                        {
                            Cancel();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Не найден код компьютера.");
                        }
                    }
                    catch (Exception err)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Ошибка чтения файла {0}. Ошибка: {1}", Const.SYS.file, err.Message);
                    }
                }

            }
            else
            {
                Cancel();
                Console.Write("Поиск файла {0}", Const.INFILE.regserv);
                if (File.Exists(Path.Combine(Const.APP.dir, Const.INFILE.regserv)))
                {
                    ok();
                    if (checkRegInfoFile())
                    {
                        XmlDocument xdoc = new XmlDocument();
                        try
                        {
                            xdoc.Load(Const.SYS.fullpath);
                            Console.Write("Чтение информации о сервере регистрации: ");
                            string[] resrv = GetServer();
                            if (resrv != null)
                            {
                                ok();
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("{0}:{1}", resrv[0], resrv[1]);

                                if (xdoc.SelectSingleNode(String.Format("//*[text() = '{0}']", resrv[0])) == null)
                                {
                                    XmlNode regItem = xdoc.SelectSingleNode("/Registration");
                                    if (regItem != null)
                                    {
                                        bool addRegItems = true;
                                        if (xdoc.SelectSingleNode("/Registration/RegistrationItems") != null)
                                        {
                                            addRegItems = false;
                                            regItem = xdoc.SelectSingleNode("/Registration/RegistrationItems");
                                        }


                                        using (XmlWriter writer = regItem.CreateNavigator().AppendChild())
                                        {
                                            if (addRegItems) writer.WriteStartElement("RegistrationItems");
                                            writer.WriteStartElement("RegistrationItem");
                                            writer.WriteElementString("ActivationCode", "");
                                            writer.WriteElementString("RegistrationType", "1");
                                            writer.WriteElementString("ServerAddress", resrv[0]);
                                            writer.WriteElementString("ServerPort", resrv[1]);
                                            writer.WriteElementString("Inactive", "0");
                                        }

                                        Console.Write("Запись сервера регистрации в файл");
                                        try
                                        {
                                            xdoc.Save(Const.SYS.fullpath);
                                            ok();
                                        }
                                        catch (Exception err)
                                        {
                                            Cancel();
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("Во время записи произошла ошибка {0}.", err.Message);
                                        }

                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Не найден раздел для записи информации о сервере ");
                                    }
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Запись нового сервера активации на требуется");
                                }
                            }
                            else
                            {
                                Cancel();
                            }
                        }
                        catch (Exception err)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ошибка чтения файла {0}. Ошибка: {1}", Const.SYS.file, err.Message);
                        }
                    }
                    else
                    {
                        Cancel();
                    }
                }
            }
            Console.ResetColor();
            Console.WriteLine("Нажмите любую кнопку для закрытия.");
            Console.ReadLine();
        }

        private static void ok()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\tOK\r\n");
            Console.ResetColor();
        }

        private static void Cancel()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\tОтмена\r\n");
            Console.ResetColor();
        }

        private static bool checkRegInfoFile()
        {
            Console.Write("Поиск информации о регистрации");
            if (File.Exists(Const.SYS.fullpath))
            {
                ok();
                return true;
            }
            else
            {
                Cancel();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("В каталоге {0} не найден файл с регистрацией программ СТМ {1}.", Const.SYS.path, Const.SYS.file);
                return false;
            }
        }

        private static string GetActivation(string val)
        {
            string result = "", line;
            try
            {
                StreamReader file = new StreamReader(Path.Combine(Const.APP.dir, Const.INFILE.reglist));
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Split(Const.APP.delimeters)[0] == val)
                    {
                        file.Close();
                        return line.Split(Const.APP.delimeters)[1];
                    }
                }

                file.Close();
            }
            catch
            {
                return String.Empty;
            }


            return result;
        }
        private static string[] GetServer()
        {
            try
            {
                StreamReader file = new StreamReader(Path.Combine(Const.APP.dir, Const.INFILE.regserv));
                string line = file.ReadLine();
                file.Close();
                return line.Split(Const.APP.delimeters);
            }
            catch
            {
                return null;
            }
        }
    }
}
