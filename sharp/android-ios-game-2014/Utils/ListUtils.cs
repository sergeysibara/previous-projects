namespace System.Collections.Generic
{
    public static class ListUtils
    {
        /// <summary>
        /// Создает список из текущего, в котором сначало идут элементы от startIndex, а потом элементы от 0 до startIndex. startIndex исключается.
        /// т.е. если есть список содержащий 1,2,3,4,5 и передан индекс =2 (индекс от 0), то вернется список 4,5,1,2
        /// </summary>
        public static List<T> CreateListFrom<T>(int startIndex, List<T> list)
        {
            startIndex = Math.Min(Math.Max(startIndex, 0), list.Count - 1);//аналог clamp

            var retList = new List<T>();

            for (int i = startIndex + 1; i < list.Count; i++)
                retList.Add(list[i]);

            for (int i = 0; i < startIndex; i++)
                retList.Add(list[i]);

            return retList;
        }

        /// <summary>
        /// Создает список из текущего, в котором сначало идут элементы от startIndex до 0, а потом элементы от последнего до startIndex. startIndex исключается.
        /// т.е. если есть список содержащий 1,2,3,4,5 и передан индекс =2(индекс от 0), то вернется список 2,1,5,4
        /// </summary>
        public static List<T> CreateReversedListFrom<T>(int startIndex, List<T> list)
        {
            var retList = CreateListFrom(startIndex, list);
            retList.Reverse();
            return retList;

        }
    }
}
