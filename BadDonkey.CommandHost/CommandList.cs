using System.Collections.Generic;

namespace BadDonkey.CommandHost
{
    public class CommandList : List<ICommand>
    {
        private IEnumerator<ICommand> _x;

        public void Reset()
        {
            _x = GetEnumerator();
        }

        public ICommand Current => _x.Current;

        public bool Next() => _x.MoveNext();
    }
}
