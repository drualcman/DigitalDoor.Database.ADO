namespace Database.ADO.BusinesObjects.Helpers;
internal static class FilesHelpers
{
    public static string CheckFolder(string carpeta)
    {
        if(!string.IsNullOrEmpty(carpeta))
        {
            carpeta = carpeta.Trim();        //delete spaces before and end
                                             //comprobar que la carpeta tiene la \ al final
            if(carpeta.Substring(carpeta.Length - 1, 1) != "\\") carpeta = carpeta + "\\";
            //comprobar que el primer caracter no es \
            if(carpeta.Substring(0, 1) == "\\") carpeta = carpeta.Remove(0, 1);
            return carpeta;
        }
        else return carpeta;
    }

    public static string SaveStringToFile(string fileName, string stringToSave, string folder = "", bool dynamicName = false, string prefix = "")
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] file = encoding.GetBytes(stringToSave);
        return SaveBytesToFile(fileName, file, folder, dynamicName, prefix);
    }
    public static string SaveBytesToFile(string fileName, byte[] bytes, string folder = "", bool dynamicName = false, string prefix = "")
    {
        string strRuta;
        string guardar = "";

        folder = CheckFolder(folder);

        //comprobar que el archivo no existe en la web
        if(dynamicName == true)
        {
            do
            {
                //el nombre existe en el servidor, cambiar el nombre del archivo
                //tantas veces como sea necesario para poder almacenar el archivo
                guardar = prefix + RandomFileName().Trim() + Path.GetExtension(fileName);
                strRuta = Path.Combine(folder, guardar);
            } while(File.Exists(strRuta));
        }
        else
        {
            if(string.IsNullOrEmpty(prefix)) guardar = fileName;
            else guardar = prefix + fileName;
        }
        strRuta = folder + guardar;

        try
        {
            using FileStream save = new FileStream(strRuta, FileMode.OpenOrCreate, FileAccess.Write);
            save.Write(bytes, 0, bytes.Length);
            save.Close();
            return guardar;
        }
        catch(Exception ex)
        {
            string info = "Can't save file: " + fileName +
                          " \r\n In Folder: " + strRuta +
                          " \r\n " + ex.Message;
            throw new ArgumentException(info);
        }
    }

      private static string RandomFileName()
        {
            string randomNum = String.Empty;
            Random autoRand = new Random();

            byte h;

            for(h = 1; h <= 5; h++)
            {
                int i_letra = Convert.ToInt32(autoRand.Next(65, 90));
                string letra = Convert.ToString(i_letra);
                randomNum += letra;
                for(int x = 0; x <= 2; x++)
                {
                    randomNum += Convert.ToInt32(autoRand.Next(0, 9)).ToString();
                }
                for(int x = 0; x <= 2; x++)
                {
                    i_letra = Convert.ToInt32(autoRand.Next(65, 90));
                    letra = Convert.ToString(i_letra);
                    randomNum += letra;
                }
                letra = Convert.ToString(Convert.ToInt32(autoRand.Next(65, 90)));
                randomNum += letra;
            }

            return randomNum;
        }
}
