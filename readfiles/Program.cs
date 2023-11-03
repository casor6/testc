using System;
using System.IO;
using System.Runtime.ConstrainedExecution;

public class Program
{//leer la informacion como una matriz de datos
    public static void Main()
    {
        bool finalizado = false;
        string path = "TiendaUsuarios.txt";//archivo para guardar los datos de usuario
        string pathJuegos = "TiendaJuegos.txt";//archivo para guardar los datos de los juegos
        if (!File.Exists(path))//si no existe el archivo de usuarios, lo creamos aqui
        {
            Console.WriteLine("Archivo inexistente");
            StreamWriter sw = File.CreateText(path);
            sw.WriteLine("idUsuario, nombreUsuario, saldo, puntos, saldoMaximo, compras, juegos, estatus");
            sw.Close();
            Console.WriteLine("Archivo Creado");
        }

        string menu = "Ingresa la opción deseada:\n1-Agregar usuario\n2-Dar de baja usuario\n3-Reembolsar\n4-Juegos\n5-Recargar\n6-Comprar Juego\nQ-Finalizar";
        
        while(!finalizado)
        {
            Console.WriteLine(menu);
            string entrada = Console.ReadLine();
            switch (entrada)
            {
                case "1":
                    NuevoUsuario(path);
                    break;
                case "2":
                    EliminarUsuario(path);
                    break;
                case "3":
                    Reembolsar(path);
                    break;
                case "4":
                    ListarJuegos(pathJuegos);
                    break;
                case "5":
                    Recargar(path);
                    break;
                case "Q":
                    Console.Write("Adios");
                    finalizado = true;
                    break;
                default:
                    Console.WriteLine("No existe esa opcion");
                    
                    break;
            }
        }
        
    }

    public static void NuevoUsuario(string path)
    {
        string[] cabeceras = { "Id de Usuario", "Nombre de Usuario", "Saldo inicial", "Puntos iniciales", "Saldo Maximo" };
        string[] datos = new string[8];//creamos un arreglo donde vamos ir ingresando los datos del nuevo usuario, del tamano de las cabeceras
        bool existeUsuario = false;
        for (int i = 0; i < cabeceras.GetLength(0); i++)//recorremos las cabeceras
        {
            Console.WriteLine("Ingresa " + cabeceras[i]);//solicitamos los datos de acuerdo a la cabecera
            datos[i] = Console.ReadLine();//asignamos el valor que ingresamos a cada posicion del arreglo datos
            if (i == 0 && ValidarUsuario(datos[i], path))//validamos con el Id de Usuario si ya existe
            {
                Console.Write("El usuario ya existe");
                existeUsuario = true;//si ya existia el usuario, cambiamos la variable para saber que existe
                break;
            }
        }
        if(!existeUsuario)//si no existe el usuario, continuamos para guardar la informacion en la BD
        {
            datos[5] = "[]";//posicion de las compras, un arreglo vacio por que es nuevo
            datos[6] = "[]";//posicion de los juegos comprados, un arreglo vacio por que es nuevo
            datos[7] = "Activo";//estatus del usuario, Activo por que es nuevo

            StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
            string contenido = sr.ReadToEnd();//asiganmos los valores a una nueva variable "contenido"
            sr.Close();
            string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
            string[,] db = new string[filas.Length, 8];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
            GuardarDatosUsuario(datos, path);
        }
        else
        {

        }
        
    }

    public static void GuardarDatosUsuario(string[] datos, string path)
    {
        StreamWriter mod = new StreamWriter(path, true);//obtenemos la BD y lo que escribamos se agrega a la ultima linea
            
        for (int j = 0; j < datos.GetLength(0); j++)//recorremos los datos que vamos a guardar del nuevo usuario
        {
            mod.Write(datos[j]);//escribimos la informacion que agregamos del usuario
            mod.Write(',');//vamos separando cada una de la informacion agregada
        }
        mod.Write('\n');//agregamos al final un salto de linea para que sea para el siguiente registro
        mod.Close();//cerramos el archivo
    }

    public static bool ValidarUsuario(string idUsuario, string path)
    {
        StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
        string contenido = sr.ReadToEnd();//asiganmos los valores a una nueva variable "contenido"
        sr.Close();
        string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
        string[,] db = new string[filas.Length, 8];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
        bool existe = false;
        for (int i = 1; i < db.GetLength(0)-1; i++)//recorremos todos los registros de ls BD
        {
            string[] data = filas[i].Split(",");//dividimos cada linea de la BD para obtener los datos de cada registro
            if (data[0] == idUsuario)//verificamos con la primer posicion(idusuario) con el idusuario que se esta tratando de registrar
            {
                existe = true;//si ya existia, cambiamos la variable a true
                break;//salimos del for
            }
        }
        return existe;//retornamos el valor para saber si continua con el registro o no
    }

    public static void EliminarUsuario(string path) {
        string menu = "Ingresa la opción deseada:\n1-Dar de baja mediante Id\nQ-Regresar al menu principal";
        bool finalizar = false;
        while(!finalizar)
        {
            Console.WriteLine(menu);
            string opcion = Console.ReadLine();
            switch (opcion)
            {
                case "1":
                    Console.WriteLine("Ingresa el Id de Usuario");
                    string idUsuario = Console.ReadLine();
                    if(ValidarUsuario(idUsuario, path))
                    {
                        CambiarEstatus(idUsuario, path);
                    }
                    else
                    {
                        Console.WriteLine("No existe el usuario con ese Id");
                    }
                    break;
                case "Q":
                    finalizar = true;
                    break;
            }
        }
    }

    public static void CambiarEstatus(string idUsuario, string path)
    {
        StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
        string contenido = sr.ReadToEnd();//asignmos los valores a una nueva variable "contenido"
        sr.Close();
        string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
        string[,] db = new string[filas.Length, 8];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
        for (int i = 0; i < db.GetLength(0) - 1; i++)// llenamos la nueva variable con los datos de la BD
        {
            string[] columnas = filas[i].Split(",");
            for (int j = 0; j < db.GetLength(1); j++)
            {
                db[i, j] = columnas[j].Trim();
            }
        }
        for (int i = 0; i < db.GetLength(0); i++)//recorremos todos los registros de la BD sin contar las cabeceras
        {
            string[] data = filas[i].Split(",");//dividimos cada linea de la BD para obtener los datos de cada registro
            if (data[0] == idUsuario)//verificamos con la primer posicion(idusuario) con el idusuario que se esta tratando de dar de baja
            {
                int saldo = int.Parse(data[2]);//obtenemos el saldo y lo convertimos a int
                if (saldo > 0)//validamos si tiene saldo
                {
                    Console.WriteLine("El usuario cuenta con " + saldo + " y no se puede dar de baja. Reembolsa su dinero");//si tiene saldo, no lo podemos dar de baja
                }
                else
                {
                    db[i, 7] = "Inactivo";//cambiamos el estatus de acuerdo a la posicion de la cabecera ESTATUS
                    GuardarDatosEstatus(db, path);
                    Console.WriteLine("Estatus cambiado para el usuario " + db[i,0]);
                }
                break;
            }
        }
    }

    public static void Reembolsar(string path) {
        Console.WriteLine("Ingresa el Id de Usuario");
        string idUsuario = Console.ReadLine();
        if (ValidarUsuario(idUsuario, path))
        {
            StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
            string contenido = sr.ReadToEnd();//asignmos los valores a una nueva variable "contenido"
            sr.Close();
            string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
            string[,] db = new string[filas.Length, 8];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
            for (int i = 0; i < db.GetLength(0) - 1; i++)// llenamos la nueva variable con los datos de la BD
            {
                string[] columnas = filas[i].Split(",");
                for (int j = 0; j < db.GetLength(1); j++)
                {
                    db[i, j] = columnas[j].Trim();
                }
            }
            for (int i = 0; i < db.GetLength(0); i++)//recorremos todos los registros de la BD sin contar las cabeceras
            {
                string[] data = filas[i].Split(",");//dividimos cada linea de la BD para obtener los datos de cada registro
                if (data[0] == idUsuario)//verificamos con la primer posicion(idusuario) con el idusuario que se esta tratando de reembolsar
                {
                    int saldo = int.Parse(data[2]);//obtenemos el saldo y lo convertimos a int
                    Console.WriteLine("El usuario tiene " + saldo + " de saldo.\nReembolsar ese dinero?\n1. Si\n2. No");
                    string respuesta = Console.ReadLine();
                    if (respuesta == "1")
                    {
                        db[i, 2] = "0";//cambiamos el saldo a cero de acuerdo a la posicion de la cabecera SALDO
                        GuardarDatosEstatus(db, path);
                        Console.WriteLine("Saldo reembolsado del usuario " + db[i, 0]);
                    }
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("No existe el usuario con ese Id");
        }
    }
    public static void Recargar(string path)
    {
        Console.WriteLine("Ingresa el Id de Usuario");
        string idUsuario = Console.ReadLine();
        if (ValidarUsuario(idUsuario, path))
        {
            StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
            string contenido = sr.ReadToEnd();//asignmos los valores a una nueva variable "contenido"
            sr.Close();
            string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
            string[,] db = new string[filas.Length, 8];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
            for (int i = 0; i < db.GetLength(0) - 1; i++)// llenamos la nueva variable con los datos de la BD
            {
                string[] columnas = filas[i].Split(",");
                for (int j = 0; j < db.GetLength(1); j++)
                {
                    db[i, j] = columnas[j].Trim();
                }
            }
            for (int i = 0; i < db.GetLength(0); i++)//recorremos todos los registros de la BD sin contar las cabeceras
            {
                string[] data = filas[i].Split(",");//dividimos cada linea de la BD para obtener los datos de cada registro
                if (data[0] == idUsuario)//verificamos con la primer posicion(idusuario) con el idusuario que se esta tratando de recargar
                {
                    int saldo = int.Parse(data[2]);//obtenemos el saldo y lo convertimos a int
                    Console.WriteLine("El usuario tiene " + saldo + " de saldo.\nCuanto dinero va recargar?");
                    string nuevosaldo = Console.ReadLine();
                    Console.WriteLine("Vas a recargar " + nuevosaldo + "\n La cantidad es correcta?\n1.Si\n2.No");
                    string respuesta = Console.ReadLine();
                    if (respuesta == "1")
                    {
                        int nuevo = saldo + Convert.ToInt32(nuevosaldo);
                        db[i, 2] = Convert.ToString(nuevo);//Sumamos el saldo anterior mas la recarga nueva
                        GuardarDatosEstatus(db, path);
                        Console.WriteLine("Se recargo al usuario " + db[i, 0]);
                    }
                    break;
                }
            }
        }
        else
        {
            Console.WriteLine("No existe el usuario con ese Id");
        }
    }
    public static void GuardarDatosEstatus(string[,] db, string path)//guardamos toda la base de datos con los datos actualizados
    {
        StreamWriter mod = File.CreateText(path);
        for (int i = 0; i < db.GetLength(0) - 1; i++)
        {
            for (int j = 0; j < db.GetLength(1); j++)
            {
                if (j == db.GetLength(1) - 1)
                {
                    mod.Write(db[i, j]);
                    mod.Write('\n');
                }
                else
                {
                    mod.Write(db[i, j]);
                    mod.Write(',');
                }
            }
        }
        mod.Close();
    }
    public static void ListarJuegos(string path)
    {
        StreamReader sr = File.OpenText(path);//leemos el archivo de la BD de juegos
        string contenido = sr.ReadToEnd();//asignmos los valores a una nueva variable "contenido"
        sr.Close();
        string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
        string[,] db = new string[filas.Length, 4];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idjuego, nombre, etc)
        for (int i = 0; i < db.GetLength(0) - 1; i++)// llenamos la nueva variable con los datos de la BD
        {
            string[] columnas = filas[i].Split(",");
            for (int j = 0; j < db.GetLength(1); j++)
            {
                db[i, j] = columnas[j].Trim();
            }
        }
        for (int i = 0; i < db.GetLength(0); i++)
        {
            for (int j = 0; j < db.GetLength(1); j++)
            {
                Console.Write(db[i, j]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}