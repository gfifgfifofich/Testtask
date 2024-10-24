using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
	Класс для обращения с данными как в классах, с возможностью сохранения и загрузки с файла.
	Писал его для игрового движка, оттуда и названия (Save, Load, DataStorage, ect.)
	Данная имплементация - грубый перевод с C++, однако Dictionary работает полностью как std::map, 
	поэтому сам класс полностью безопасен по памяти, null значениям и прочему
 */

namespace ConsoleApp1
{
	internal class DataStorage
	{
		public  Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();

		public void AddObject(string ObjectName)
        {
            ObjectName = ObjectName.Trim();
            data.TryAdd(ObjectName, new Dictionary<string, string>());
		}
		public void AddProperty(string ObjectName, string Property, string value = "")
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            data[ObjectName].TryAdd(Property, value);
		}

        public Dictionary<string, string> GetObject(string ObjectName)
        {
            ObjectName = ObjectName.Trim();
            // если нету значения или объекта, создать пустой 
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);

            return data[ObjectName];
		}

        public string GetProperty(string ObjectName, string Property)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            // если нету значения или объекта, создать пустой 
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);
            if (!data[ObjectName].ContainsKey(Property))
                AddProperty(ObjectName, Property);

            string prop = data[ObjectName][Property];
            prop = prop.Trim();
			return prop;
		}
        public int GetPropertyAsInt(string ObjectName, string Property)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            return Convert.ToInt32(GetProperty(ObjectName, Property));
		}
        public float GetPropertyAsFloat(string ObjectName, string Property)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            return Convert.ToSingle(GetProperty(ObjectName, Property));
		}
        public bool GetPropertyAsBool(string ObjectName, string Property)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            return Convert.ToBoolean(GetProperty(ObjectName, Property));
		}

        public void SetProperty(string ObjectName, string Property, string value = "")
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            // ибо Dictionary не создаёт значения автоматически
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);
            if (!data[ObjectName].ContainsKey(Property))
                AddProperty(ObjectName, Property);
            data[ObjectName][Property] = value;
		}
        public void SetProperty(string ObjectName, string Property, bool value)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            // ибо Dictionary не создаёт значения автоматически
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);
            if (!data[ObjectName].ContainsKey(Property))
                AddProperty(ObjectName,Property);

            data[ObjectName][Property] = Convert.ToString(value);
		}
        public void SetProperty(string ObjectName, string Property, int value)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            // ибо Dictionary не создаёт значения автоматически
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);
            if (!data[ObjectName].ContainsKey(Property))
                AddProperty(ObjectName, Property);

            data[ObjectName][Property] = Convert.ToString(value);
		}
        public void SetProperty(string ObjectName, string Property, float value)
        {
            ObjectName = ObjectName.Trim();
            Property = Property.Trim();
            // ибо Dictionary не создаёт значения автоматически
            if (!data.ContainsKey(ObjectName))
                AddObject(ObjectName);
            if (!data[ObjectName].ContainsKey(Property))
                AddProperty(ObjectName, Property);

            data[ObjectName][Property] = Convert.ToString(value);
		}

        public void Save(string filename)
		{
			StreamWriter file = new StreamWriter(filename);
			foreach (var e in data)
			{
				file.Write(e.Key + "\n" + "{" );

				foreach (var p in e.Value)
				{
					file.Write("\n	" + p.Key + " " + p.Value + "");
				}

				file.Write("\n}\n");
			}
			file.Close();
		}
        public void Load(string filename)
		{
			data.Clear();
            
            //Создать файл если его небыло до этого
            StreamWriter file = new StreamWriter(filename,true);
            file.Close();

            StreamReader sr = new StreamReader(filename);
			

			string lastObject = "NULL";

			while (!sr.EndOfStream)
			{
				string line = "";
				line = sr.ReadLine(); // VS пишет что возможен нулл, но нулл невозможен изза условия в цикле

				if(line != null && line.Length == 0)
					continue;

				if (line[0] == '{')  // аналогично
					continue;

				// reset last object, "exit object"
				if (line[0] == '}' && lastObject != "NULL")
				{
					lastObject = "NULL";
                    continue;
				}
				// "enter object"
				if (line[0] != '{' && line[0] != '}' && lastObject == "NULL")
				{
					lastObject = "";
                    line = line.Trim();
                    lastObject = line;
					if (lastObject != "")
						AddObject(lastObject);
					continue;
				}

				if (lastObject != "NULL")
				{
					string property = "";
					string value = "";

                    line = line.Trim();
                    int i = 0;
                    while (i < line.Length && line[i] != ' ')
                    {
                        property += line[i];
                        i++;
                    }
                    i++;// skip space;
                    while (i < line.Length)
                    {
                        value += line[i];
                        i++;
                    }
                    AddProperty(lastObject, property, value);
					continue;
				}
			}
            sr.Close();
        }
	}
}
