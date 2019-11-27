using System;
using System.Collections.Generic;

namespace simple
{
    class Program
    {
        static void Main(string[] args)
        {
            TextInput sk = new NumberInput();
            sk.add("t");
            sk.add("u");
            sk.add("n");
            sk.add("d");
            sk.add("e");
            sk.add("1");
            sk.add("0");
            Console.WriteLine(sk.getValue());
            Console.ReadLine();
        }
    }

    public class TextInput
    {
        public List<string> _inputs = new List<string>();
        public List<int> _inputsInt = new List<int>();

        public void add(string _input)
        {
            try
            {
                
                    _inputs.Add(_input);
                
                //}
                
            }
            catch(Exception es)
            {
                Console.WriteLine(es.Message);
            }
            

        }

        public void add(int _input)
        {

            if (_input.GetType() == typeof(string))
            {
                _inputsInt.Add(_input);
            }

        }

        public string getValue()
        {
            string _sk = "";
            foreach(var s in _inputs)
            {
               _sk += s;
            }

            return _sk;
        }
    }

    public class NumberInput : TextInput
    {
        public void add(int _input)
        {
            
                base._inputsInt.Add(_input);
          

        }
    }
}
