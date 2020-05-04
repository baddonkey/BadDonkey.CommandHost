using System.Collections.Generic;

namespace BadDonkey.CommandHost
{
    public class CommandList : List<Command>
    {
        private IEnumerator<Command> _x;

        public void Reset()
        {
            _x = GetEnumerator();
        }

        public Command Current => _x.Current;

        public bool Next() => _x.MoveNext();
    }
}
