using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Utilities 
{
    public static int mod(int x, int m)
    {
        return (x % m + m) % m;
    }
}
