#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("WpXbAtzDAPiQgGWr7XN+APjnkIWRlHL+9bQxk9Dqmdgh+bxDZ5xszPr4/ZXghK1TAUxLOPd1dHAyx+RRyJY9OXpieRp56bLk+hzvU10b3CqMX+vtcZuvxyxYjENJnChSkfFIyqjVm+sI0bbs8cZlQvPJvY9mpjAsFjMP8Ac998Bu8zTLpbeN9zVi+Me85+clw0KCSGpdoCkvrOZD/ZinkmzU4xRml769CFLNEBXVzOZLYGn57EWTn18h0IyPw+qZwwdi+b+8i7/4Ssnq+MXOweJOgE4/xcnJyc3Iy0rJx8j4SsnCykrJychiT+TdYoM5DR+9MYnG48ZbvYUnaEJGmCZN7L3svOeJlBvcWtSXMP66kG/topzTB9e1hIh5gj4YUcrLycjJ");
        private static int[] order = new int[] { 12,5,13,3,9,8,10,9,13,11,12,13,12,13,14 };
        private static int key = 200;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
