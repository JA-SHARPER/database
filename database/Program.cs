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
                
                Console.WriteLine("Opcion: \n1.Insert \n2.DELETE \n3.UPDATE \n4.SELECT");
                int opc = Convert.ToInt32(Console.ReadLine());
                List<Person> listp = new List<Person>();
                int id;

                switch (opc)
                {
                    case 1:
                        Console.WriteLine("Parametros a ingresar, nombre");
                        string name = Console.ReadLine();
                        Console.WriteLine("Parametros a ingresar, cedula");
                        string cedula = Console.ReadLine();
                        Console.WriteLine("Parametros a ingresar (id)");
                        int ID = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine("mete el path de la foto");
                        string path = Console.ReadLine().Replace('/', Convert.ToChar(@"\"));
                        
                        Console.WriteLine("mete la fecha de nacimiento");
                        string nac = Console.ReadLine();
                        byte[] image = System.IO.File.ReadAllBytes(path);

                        con.Open();
                        p.Insert(ID, name, cedula, image, nac, con);
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
                        string query = "select * FROM Person";
                        SqlCommand command = new SqlCommand(query, con);
                        con.Open();
                        var reader = command.ExecuteReader();
                        List<object> listPerson = new List<object>();
                        int count = 0;
                        while (reader.Read())
                        {
                            listPerson.Add(reader);
                            Console.WriteLine(listPerson[count]);
                            count++;
                            
                        }
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

        public Person()
        {

        }

        public void Insert(int ID, string name, string cedula, byte[] image, string nac ,SqlConnection con)
        {
            string sentence = $@"INSERT INTO Person ([ID_People]
                          ,[nombre]
                          ,[cedula]
                          ,foto  
                          ,[nacimiento])
                          VALUES (@ID_People, @nombre, @cedula, @foto, @nacimiento)";
            //string sentence = $"INSERT INTO Person VALUES ({ID}, {name}, {cedula}, {foto}, {nac})";
            SqlCommand command = new SqlCommand(sentence, con);
            
            command.Parameters.AddWithValue("ID_People", ID);
            command.Parameters.AddWithValue("nombre", name);
            command.Parameters.AddWithValue("cedula", cedula);
            command.Parameters.AddWithValue("foto", image);
            command.Parameters.AddWithValue("nacimiento", nac);

            command.ExecuteNonQuery();
        }

        public void Delete(int ID, SqlConnection con)
        {
            string sentence = $"DELETE FROM Person WHERE {ID} = Person.ID_People";
            SqlCommand command = new SqlCommand(sentence, con);
            command.Parameters.AddWithValue("ID", ID);
            command.ExecuteNonQuery();
        }

        public void Update(Person p, SqlConnection con)
        {
            Person person = new Person();
            person.Nombre = p.Nombre;
            person.ID = p.ID;
            person.Cedula = p.Cedula;
            person.Image = p.Image;
            person.Nac = p.Nac;

            string sentence = $"UPDATE Person " +
                              $"SET Person.nombre = @nombre, Person.cedula = @cedula, foto = @image, nacimiento = @nac " +
                              $"WHERE {person.ID} = Person.ID_People";
            SqlCommand command = new SqlCommand(sentence, con);
            command.Parameters.AddWithValue("nombre", person.Nombre);
            command.Parameters.AddWithValue("cedula", person.Cedula);
            command.Parameters.AddWithValue("image", person.Image);
            command.Parameters.AddWithValue("ID", person.ID);
            command.Parameters.AddWithValue("nac", person.Nac);

            command.ExecuteNonQuery();
        }
    }
}
