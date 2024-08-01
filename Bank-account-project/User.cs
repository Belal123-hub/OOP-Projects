using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultilevelMenuExample
{
    public class User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }

        public List<Wallet> Wallets { get; set; }                      // Update the property name to 'Wallets'

        public Wallet SelectedWallet
        {
            get { return ActiveWallet; }
        }
        public Wallet? ActiveWallet { get; set; }

    }

}
