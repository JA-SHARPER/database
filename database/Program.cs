using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace database
{
    class Program
    {
        static void Main(string[] args)
        {
            var constring = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

            SqlConnection con = new SqlConnection(constring);

            while (true)
            {
                Person p = new Person();
                Hijo hijo = new Hijo();

                Console.WriteLine("Opcion: \n1.Insert \n2.DELETE \n3.UPDATE \n4.SELECT \n5.Agregar Hijos ");
                int opc = Convert.ToInt32(Console.ReadLine());
                int id;
                switch (opc)
                {
                    case 1:
                        Console.WriteLine("Parametros a ingresar, nombre");
                        p.Nombre = Console.ReadLine();

                        Console.WriteLine("Parametros a ingresar, cedula");
                        p.Cedula = Console.ReadLine();

                        Console.WriteLine("mete el path de la foto");
                        string path = Console.ReadLine().Replace('/', Convert.ToChar(@"\"));
                        byte[] image = System.IO.File.ReadAllBytes(path);
                        p.Image = image;

                        Console.WriteLine("mete la fecha de nacimiento");
                        p.Nac = Convert.ToDateTime(Console.ReadLine());
                        
                        con.Open();
                        p.Insert(p , con);
                        con.Close();
                        Console.WriteLine("nice");
                        break;

                    case 2:
                        Console.WriteLine("Insert the id of who you deletin)");
                        id = Convert.ToInt32(Console.ReadLine());
                        
                        con.Open();
                        p.Delete(id, con);
                        con.Close();
                        break;

                    case 3:
                        Console.WriteLine("Insert the id of who you updatin)");
                        p.ID = Convert.ToInt32(Console.ReadLine());
                        
                        Console.WriteLine("Parametros a ingresar, nombre");
                        p.Nombre = Console.ReadLine();
                        Console.WriteLine("Parametros a ingresar, cedula");
                        p.Cedula = Console.ReadLine();
                        Console.WriteLine("mete la fecha de nacimiento");
                        p.Nac = Convert.ToDateTime(Console.ReadLine());
                        Console.WriteLine("mete la path de la imagen");
                        p.Image = System.IO.File.ReadAllBytes(Console.ReadLine());

                        con.Open();
                        p.Update(p, con);
                        con.Close();
                        break;

                    case 4:
                        string query = "select * FROM Persona";
                        SqlCommand command = new SqlCommand(query, con);
                        con.Open();
                        List<Hijo> listHijo = hijo.GetAllHijos();

                        var reader = command.ExecuteReader();
                        List<Person> listPerson = new List<Person>();
                        int count = 0;
                        
                        while (reader.Read())
                        {
                            listPerson.Add(new Person() {
                                ID = Convert.ToInt32(reader.GetValue(0)),
                                Nombre = reader.GetValue(1).ToString(),
                                Cedula = reader.GetValue(2).ToString(),
                                Nac = Convert.ToDateTime(reader.GetValue(3))
                            });
                            var GetHijosdePadre = (from person in listPerson
                                         join hijito in listHijo
                                         on person.ID equals hijito.ID_Persona
                                         where listPerson[count].ID == hijito.ID_Persona
                                         select hijito.Nombre).FirstOrDefault();

                            Console.WriteLine($"{listPerson[count].ID} | {listPerson[count].Nombre} |" +
                                $" {listPerson[count].Cedula} | {listPerson[count].Nac}");
                            Console.WriteLine($"El Primer hijo de este individuo es {GetHijosdePadre}\n");

                            count++;
                        }
                        con.Close();
                        break;

                    case 5:
                        Console.WriteLine("Parametros a ingresar pal chamako, nombre");
                        hijo.Nombre = Console.ReadLine();
                        Console.WriteLine("De quien es el chamakito??? ()");
                        hijo.ID_Persona = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("mete la fecha de nacimiento");
                        hijo.Nac = Convert.ToDateTime(Console.ReadLine());

                        con.Open();
                        hijo.Insert(hijo, con);
                        con.Close();

                        break;



                    default:
                        Console.WriteLine("seletcione algo");
                        break;
                }
                Console.ReadLine();
            }
        }

    }


    class Person
    {
        public int ID { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public byte[] Image { get; set; }
        public DateTime Nac { get; set; }
        public int Hijo { get; set; }

        public Person()
        {

        }

        public void Insert(Person p, SqlConnection con)
        {
            string sentence = $@"INSERT INTO Persona ([nombre]
                          ,[cedula]
                          ,foto  
                          ,[fechaNac])
                          VALUES (@nombre, @cedula, @foto, @nacimiento)";
            SqlCommand command = new SqlCommand(sentence, con);
            
            command.Parameters.AddWithValue("nombre", p.Nombre);
            command.Parameters.AddWithValue("cedula", p.Cedula);
            command.Parameters.AddWithValue("foto", p.Image);
            command.Parameters.AddWithValue("nacimiento", p.Nac);

            command.ExecuteNonQuery();
       }

        public void Delete(int id, SqlConnection con)
        {
            string sentence = $"DELETE FROM Persona WHERE {id} = Person.ID_People";
            SqlCommand command = new SqlCommand(sentence, con);
            command.Parameters.AddWithValue("ID_Persona", id);
            command.ExecuteNonQuery();
        }

        public void Update(Person p, SqlConnection con)
        {
            Person person = new Person();
            person.Nombre = p.Nombre;
            person.Cedula = p.Cedula;
            person.Image = p.Image;
            person.Nac = p.Nac;

            string sentence = $"UPDATE Persona " +
                              $"SET Persona.nombre = @nombre, cedula = @cedula, foto = @image, fechaNac = @nac " +
                              $"WHERE {person.ID} = Person.ID_People";
            SqlCommand command = new SqlCommand(sentence, con);
            command.Parameters.AddWithValue("nombre", person.Nombre);
            command.Parameters.AddWithValue("cedula", person.Cedula);
            command.Parameters.AddWithValue("image", person.Image);
            command.Parameters.AddWithValue("nac", person.Nac);

            command.ExecuteNonQuery();
        }
    }
    class Hijo
    {
        public int ID_Hijo { get; set; }
        public string Nombre { get; set; }
        public DateTime Nac { get; set; }
        public int ID_Persona { get; set; }

        public void Insert(Hijo hijo, SqlConnection con)
        {
            string sentence = $@"INSERT INTO Hijo(nombre, fechaNacimiento, ID_Persona)
                          VALUES (@nombre, @nacimiento, @padre)";
            SqlCommand command = new SqlCommand(sentence, con);

            command.Parameters.AddWithValue("nombre", hijo.Nombre);
            command.Parameters.AddWithValue("padre", hijo.ID_Persona);
            command.Parameters.AddWithValue("nacimiento", hijo.Nac);

            command.ExecuteNonQuery();
        }

        public List<Hijo> GetAllHijos()
        {
            var constring = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

            SqlConnection con = new SqlConnection(constring);
            string query = "select * FROM Hijo";
            SqlCommand command = new SqlCommand(query, con);
            
            con.Open();
            var reader = command.ExecuteReader();
            List<Hijo> listHijo = new List<Hijo>();

            while (reader.Read())
            {
                listHijo.Add(new Hijo()
                {
                    ID_Hijo = Convert.ToInt32(reader.GetValue(0)),
                    Nombre = reader.GetValue(1).ToString(),
                    Nac = Convert.ToDateTime(reader.GetValue(2)),
                    ID_Persona = Convert.ToInt32(reader.GetValue(3))
                });
            }
            con.Close();
            return listHijo;
        }
    }
}
