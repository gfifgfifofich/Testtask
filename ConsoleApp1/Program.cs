using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ConsoleApp1
{
	static class Program
	{
		// Функция в конечном варианте не потребовалась. Может пригодиться при создании данных. Использована только в логах, для одинакового формата с остальным тз
		public static string TimeToFormatedString(DateTime dt)
		{
			string s = Convert.ToString(dt.Year) + "-" + Convert.ToString(dt.Month) + "-" + Convert.ToString(dt.Day);
			string[] ss = dt.ToString().Split(' ');
			s += " " + ss[1];
			return s;
		}

		static void Main(string[] argv)
		{            
			DataStorage ConfigDS = new DataStorage();
			ConfigDS.Load("./Config.txt");
			bool showHelp = false;
			int argvIter = 0;
			foreach (string str in argv)
			{
				if (str == "-h")
					showHelp = true;
				if (str == "-l" && argvIter + 1 < argv.Length)
					ConfigDS.SetProperty("Config", "_deliveryLog",argv[argvIter + 1]);
				if (str == "-o" && argvIter + 1 < argv.Length)
					ConfigDS.SetProperty("Config", "_deliveryOrder", argv[argvIter + 1]);
				if (str == "-i" && argvIter + 1 < argv.Length)
					ConfigDS.SetProperty("Config", "_deliveryInput", argv[argvIter + 1]);
				if (str == "-d" && argvIter + 1 < argv.Length)
					ConfigDS.SetProperty("Config", "_deliveryDistrict", argv[argvIter + 1]);
				if (str == "-fdt" && argvIter + 2 < argv.Length)
					ConfigDS.SetProperty("Config", "_deliveryTime", argv[argvIter + 1] + " " + argv[argvIter + 2]);
				argvIter++;
			}

            DataStorage inputDS = new DataStorage();
            DataStorage outputDS = new DataStorage();
            DataStorage LogDS = new DataStorage(); // логи лучше было бы выводить просто в файл.

            string _deloveryLog = ConfigDS.GetProperty("Config", "_deliveryLog");
            try
            {
                // для сохранения предыдущих можно предварительно загрузить предыдущий файл
                //LogDS.Load(_deloveryLog);
                // для чистоты данной реализации, логи стираются после каждого запуска.
                LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(),"	Deleted logs");
                LogDS.Save(_deloveryLog);
            }
            catch
            {
                Console.WriteLine("Некорректный путь к файлу логов");
                return;
            }
            string _deliveryOrder = ConfigDS.GetProperty("Config", "_deliveryOrder");
            try
            {
                outputDS.Save(_deliveryOrder);
                LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	cleared output");
                LogDS.Save(_deloveryLog);
            }
            catch
            {
                Console.WriteLine("Некорректный путь к файлу вывода фильтрации");
                return;
            }
            string _deliveryInput = ConfigDS.GetProperty("Config", "_deliveryInput");
            try
            {
                inputDS.Load(_deliveryInput);
            }
            catch
            {
                Console.WriteLine("Некорректный путь к файлу исходный данных");
                return;
            }


            string _deliveryDistrict = ConfigDS.GetProperty("Config", "_deliveryDistrict");
			if(_deliveryDistrict.Length ==0)
			{
                Console.WriteLine("Не указан район доставки");
                return;
            }

			string _deliveryTime = ConfigDS.GetProperty("Config", "_deliveryTime");
			DateTime deliveryTime = DateTime.Now;
			try
			{
				deliveryTime = DateTime.Parse(_deliveryTime);
			}
			catch
			{
				Console.WriteLine("Некорректный формат времени/время доставки не указано");
				Console.WriteLine("Формат yyyy-MM-dd HH:mm:ss");
				return;
			}

			if (argv.Length == 1 || showHelp)
			{
				// help
				Console.WriteLine("-h 					Вывести данное сообщение");
				Console.WriteLine("-l path/to/log				Изменить путь для вывода логов");
				Console.WriteLine("-o path/to/log				Изменить путь для вывода приложения");
				Console.WriteLine("-i path/to/log				Изменить путь для файла с исходными данными");
				Console.WriteLine("-d district_name			Изменить район доставки");
				Console.WriteLine("-fdt yyyy-MM-dd HH:mm::ss		Изменить время первой доставки");
				Console.WriteLine("Запуск без параметров будет использовать последние параметры (см. Config.txt)");

				return;
			}
			ConfigDS.Save("./Config.txt");
            LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	Config saved");
            LogDS.Save(_deloveryLog);

            Console.WriteLine("Используемые параметры: ");
            Console.WriteLine("_deloveryLog = " + _deloveryLog);
            Console.WriteLine("_deliveryOrder = "+_deliveryOrder);
            Console.WriteLine("_deliveryInput = "+_deliveryInput);
            Console.WriteLine("_deliveryDistrict = "+_deliveryDistrict);
            Console.WriteLine("_deliveryTime = " + _deliveryTime);
            Console.WriteLine();


            foreach (var e in inputDS.data)
			{
				if(inputDS.GetProperty(e.Key, "District") == _deliveryDistrict)
				{
					// Валидация входных данных
                    DateTime dTime = DateTime.Now;
                    try
                    {
                        dTime = DateTime.Parse(inputDS.GetProperty(e.Key, "DeliveryTime"));
                    }
                    catch
                    {
                        Console.WriteLine("Некорректный формат времени/время доставки не указано в строке " + e.Key);
                        LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	resolved error in input data: Line " + e.Key + " skipped");
                        LogDS.Save(_deloveryLog);
                        continue;
                    }
                    if (inputDS.GetPropertyAsFloat(e.Key, "Weight") <= 0.0f)
                    {
                        Console.WriteLine("Некорректный вес " + e.Key);
                        LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	resolved error in input data: Line " + e.Key + " skipped");
                        LogDS.Save(_deloveryLog);
                        continue;
                    }
                    if (inputDS.GetProperty(e.Key, "District").Length <= 0)
                    {
                        Console.WriteLine("Некорректный район " + e.Key);
                        LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	resolved error in input data: Line " + e.Key + " skipped");
                        LogDS.Save(_deloveryLog);
                        continue;
                    }
                    double dif = dTime.Subtract(deliveryTime).TotalMinutes;
					if(dif>0.0 && dif <= 30.0)
                    {
						foreach (var p in e.Value)
							outputDS.SetProperty(e.Key, p.Key, p.Value);
                        LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	output entry added");
                        LogDS.Save(_deloveryLog);
                    }
                }
			}

			outputDS.Save(_deliveryOrder);
            Console.WriteLine("Результат фильтрации сохранён в " + _deliveryOrder);
            LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	Output saved");
            LogDS.Save(_deloveryLog);

            LogDS.SetProperty(TimeToFormatedString(DateTime.Now), DateTime.Now.TimeOfDay.ToString(), "	exiting");
            LogDS.Save(_deloveryLog);

        }
	}
}
