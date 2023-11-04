using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        string menu = "Ingresa la opción deseada:\n1-Agregar usuario\n2-Dar de baja usuario\n3-Reembolsar\n4-Juegos\n5-Recargar\n6-Comprar Juego\n7-Historial General\nQ-Finalizar";
        
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
                case "6":
                    ComprarJuego(path, pathJuegos);
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
                    GuardarDatosEnBD(db, path);
                    Console.WriteLine("Estatus cambiado para el usuario " + db[i,0]);
                    string registroHistorico = "Baja Cuentahabiente ---- ID_Cuentahabiente: " + idUsuario;//creamos lo que vamos a guardar en el historico
                    GuardarHistorial(registroHistorico);//lo guardamos en historico
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
                    int saldo = Convert.ToInt32(data[2]);//obtenemos el saldo y lo convertimos a int
                    Console.WriteLine("El usuario tiene " + saldo + " de saldo.\nReembolsar ese dinero?\n1. Si\n2. No");
                    string respuesta = Console.ReadLine();
                    if (respuesta == "1")
                    {
                        db[i, 2] = "0";//cambiamos el saldo a cero de acuerdo a la posicion de la cabecera SALDO
                        GuardarDatosEnBD(db, path);
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
                        GuardarDatosEnBD(db, path);
                        Console.WriteLine("Se recargo al usuario " + db[i, 0]);
                        string registroHistorico = "ID_usuario: " + idUsuario + " ----- Movimiento: Recarga ----- Monto: " + nuevosaldo;
                        GuardarHistorial(registroHistorico);
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
    public static void GuardarDatosEnBD(string[,] db, string path)//guardamos toda la base de datos con los datos actualizados
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
    public static void ComprarJuego(string path, string pathJuegos)
    {
        ListarJuegos(pathJuegos);//Mostramos la lista de juegos disponibles
        Console.WriteLine("Ingrese el Id del Usuario");//solicitamos el id del usuario que quiere comprar un juego
        string idUsuario = Console.ReadLine();
        if(ValidarUsuario(idUsuario, path))//validamos si existe el usuario con el id
        {
            
            bool existeJuego = false;
            string idJuego = "";
            while (!existeJuego)
            {
                Console.WriteLine("Ingrese el Id del Juego a comprar");//solicitamos el id del juego que quiere comprar
                idJuego = Console.ReadLine();
                string nombreJuego = ObtenerDatoEspecifico(idJuego, 0, pathJuegos, "juegos");//validamos si existe el juego
                if(nombreJuego != "")
                {
                    existeJuego = true;//si existe el juego, cambiamos la variable a true para que finalice el while y pueda continuar
                }
                
            }
            if (!ValidarSiYaSeCompro(idJuego, idUsuario, path))//validamos si el juego que quiere comprar ya lo habia comprado
            {
                int precioJuego = Convert.ToInt32(ObtenerDatoEspecifico(idJuego, 3, pathJuegos, "juegos"));//obtenemos el precio del juego y lo convertimos en int
                int saldoUsuario = Convert.ToInt32(ObtenerDatoEspecifico(idUsuario, 2, path, "usuarios"));//obtenemos el saldo del usuario y lo convertimos en int
                int puntosUsuario = Convert.ToInt32(ObtenerDatoEspecifico(idUsuario, 3, path, "usuarios"));//obtenemos los puntos del usuario y lo convertimos en int
                int puntosASaldo = Convert.ToInt32(Math.Round(Convert.ToDouble(puntosUsuario / 10)));//convertimos los puntos en saldo
                int saldoTotal = saldoUsuario + puntosASaldo;//juntamos los saldos
                if (saldoTotal >= precioJuego)//validamos que tenemos suficiente saldo para comprar el juego
                {
                    string nombreJuego = ObtenerDatoEspecifico(idJuego, 1, pathJuegos, "juegos");//obtenemos el nombre del juego para guardarlo en los registros de compra
                    DateTime fecha = DateTime.Now;
                    string formatoFecha = fecha.ToString("dd/MM/yyyy");//obtenemos la fecha de compra
                    string registroCompra = idJuego + "-" + formatoFecha + "-" + precioJuego;//generamos el registro de la compra para que sea del formato idjuego-fecha-precio
                    int nuevosPuntos = precioJuego / 10;//calculamos los nuevos puntos por la compra que se va hacer. son el 10% de cada compra
                    int puntosFinales = puntosUsuario;
                    if (saldoUsuario >= precioJuego)//validamos si con el puro saldo nos alcanza para comprar el juego sin los puntos
                    {
                        saldoUsuario = saldoUsuario - precioJuego;//quitamos del saldo, el total del juego
                        puntosFinales += nuevosPuntos;//sumamos los nuevos puntos a los puntos que ya teniamos
                    }
                    else//si no nos alcanza con el puro saldo, gastaremos los puntos tambien
                    {
                        int faltante = precioJuego - saldoUsuario;//vemos cuanto es el faltante para poder pagarlo con los puntos
                        int puntosUsados = (faltante - puntosASaldo) * 10;//vemos cuantos puntos son los que necesitamos para la compra
                        puntosFinales = puntosUsuario - puntosUsados + nuevosPuntos;//restamos los puntos que necesitamos a los puntos que teniamos, ahi mismo sumamos los nuevos puntos por la compra
                        saldoUsuario = 0;//como no nos alcanzaba con el puro saldo, entonces se ocupo todo el saldo y lo ponemos en cero
                        
                    }
                    GuardarDatosCompra(idUsuario, Convert.ToString(saldoUsuario), Convert.ToString(puntosFinales), registroCompra, nombreJuego, Convert.ToString(precioJuego), path);//guardamos toda la informacion de la compra
                }
                else
                {
                    Console.WriteLine("No tiene saldo suficiente, su saldo es: " + saldoTotal + ". El juego vale: " + precioJuego);
                }
            }
            else
            {
                Console.WriteLine("Ese juego ya existe en su catalogo, no se puede comprar");
            }

        }
        else
        {
            Console.WriteLine("No existe el usuario");
        }
    }

    public static void GuardarDatosCompra(string idUsuario, string saldoNuevo, string puntosNuevos, string registroCompra, string nombreJuego, string precioJuego, string path) {
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
                db[i, 2] = Convert.ToString(saldoNuevo);//asignamos el nuevo saldo
                db[i, 3] = Convert.ToString(puntosNuevos);//asignamos los nuevos puntos

                string comprasUsuario = ObtenerDatoEspecifico(idUsuario, 5, path, "usuarios");//obtenemos las compras
                string[] comprasArray = comprasUsuario.Split("]");//quitamos el ultimo corchete de las compras, para poder agreggar el nuevo registro de compra
                string separador = comprasArray[0].Length > 1 ? "??" : "";//validamos si ya tenemos comprar, para poder agregar el separador
                string concatenarCompras = comprasArray[0]+ separador + registroCompra+"]";//hacemos el nuevo string de compras
                db[i, 5] = concatenarCompras;//asignamos las compras

                string catalogoUsuario = ObtenerDatoEspecifico(idUsuario, 6, path, "usuarios");//obtenemos el catalogo
                string[] catalogoArray = catalogoUsuario.Split("]");//quitamos el ultimo corchete del catalogo, para poder agreggar el nuevo nombre del juego comprado
                string separadorCatalogo = catalogoArray[0].Length > 1 ? "??" : "";//validamos si ya tenemos juegos, para poder agregar el separador
                string concatenarCatalogo = catalogoArray[0] + separadorCatalogo + nombreJuego + "]";//hacemos el nuevo string de catalogo
                db[i, 6] = concatenarCatalogo;//asignamos el catalogo
                GuardarDatosEnBD(db, path);//guardamos la nueva informacion
                break;
            }
        }

        string registroHistorico = "ID_usuario: " + idUsuario + " ----- Movimiento: Compra(" + nombreJuego + ") ----- Monto: " + precioJuego;
        GuardarHistorial(registroHistorico);
    }
    public static bool ValidarSiYaSeCompro(string idJuego, string idUsuario, string pathUsuarios)
    {
        string comprasUsuario = ObtenerDatoEspecifico(idUsuario, 5, pathUsuarios, "usuarios");//obtenemos solo las compras de la base de datos
        string[] separarCorchete1 = comprasUsuario.Split("[");//las compras se encuentran entre corchetes. separamos para quitar los corchetes, quitamos el corchete de la izquierda
        string[] separarCochete2 = separarCorchete1[1].Split("]");//quitamos el corchete de la derecha
        string[] comprasArray = separarCochete2[0].Split("??");//al ya no tener corchetes, separamos las compras que hemos realizado, el separador es ??
        bool yaComprado = false;
        for(int i = 0; i < comprasArray.GetLength(0); i++)//recorremos todas las compras que tenemos
        {
            string[] datosCompra = comprasArray[i].Split("-");//dividimos cada compra, que las tenemos separadas por - (idjuego-fechaCompra-precio)
            if (datosCompra[0] == idJuego)//comparamos la primera posicion de la compra con el id del juego que queremos comprar
            {
                yaComprado = true;//si ya lo compramos, cambiamos la variable a true y con esto indicamos que ya se habia comprado
                break;
            }
        }
        return yaComprado;
    }
    public static string ObtenerDatoEspecifico(string idBuscar, int posicion, string path, string tipoBD)
    {
        int tamanioCabeceras = 0;
        switch(tipoBD)
        {
            case "usuarios":
                tamanioCabeceras = 8;
                break;
            case "juegos":
                tamanioCabeceras = 4;
                break;
        }
        if (ValidarUsuario(idBuscar, path))
        {
            StreamReader sr = File.OpenText(path);//leemos el archivo de la BD
            string contenido = sr.ReadToEnd();//asignmos los valores a una nueva variable "contenido"
            sr.Close();
            string[] filas = contenido.Split('\n'); //dividimos por salto de lineas, para saber cuantos registros existen en la BD
            string[,] db = new string[filas.Length, tamanioCabeceras];//creamos un arreglo bidimensional con el tamaño de los reggistros que existen y las cabeceras(idusuario, nombre, saldo, etc)
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
                if (data[0] == idBuscar)//verificamos con la primer posicion(idBuscar) con el id que se esta tratando de reembolsar
                {
                    return data[posicion];
                }
            }
        }
        else
        {
            Console.WriteLine("No existe el registro con ese Id");
            return "";
        }
        return "";
    }

    public static void GuardarHistorial(string registro)
    {
        string path = "Historico.txt";
        StreamWriter mod = new StreamWriter(path, true);//obtenemos la BD y lo que escribamos se agrega a la ultima linea

        mod.Write(registro);
        mod.Write('\n');//agregamos al final un salto de linea para que sea para el siguiente registro
        mod.Close();//cerramos el archivo
    }
}