namespace FunctionalStuff.Option
{
    public class None<T> : Option<T>
    {
        protected None()
        {
        }

        public static Option<T> Create()
        {
            return new None<T>();
        }
    }
}
