using System;

public class CQueue<T>
{
    private T[] array;
    public int length { get { return array.Length; } }
    public T dequedElement { get; private set; }
    public T enquedElement { get; private set; }
    public CQueue(int size) { array = new T[size]; }
    public void enqueTop(T element)
    {
        T lastElement = array[0];
        int count = array.Length;
        for (int i = 0; i < count - 1; i++)
        {
            array[i] = array[i + 1];
        }
        array[count - 1] = element;
        dequedElement = lastElement;
        enquedElement = element;
    }
    public void enqueBottom(T element)
    {
        int count = array.Length;
        T topElement = array[count - 1];
        for (int i = count - 1; i >= 1; i--)
        {
            array[i] = array[i - 1];
        }
        array[0] = element;
        enquedElement = element;
        dequedElement = topElement;
    }
    public void resize(int new_size, bool addOrremoveFromTop = true)            //if (isTop == false) => We are going to insert blocks at the end of the array
    {
        int previous_size = array.Length;
        if (previous_size == new_size) { CLogger.wrn("The Queue's previous size is matched with the new size"); return; }
        T[] new_array = new T[new_size];
        if (new_size > previous_size)
        {
            if (addOrremoveFromTop)
            {
                int i = 0;
                for (; i < previous_size; i++)
                    new_array[i] = array[i];
                for (i = previous_size; i < new_size; i++) { new_array[i] = default(T); }
            }
            else
            {
                int toBeIncluded = new_size - previous_size;
                int i = 0;
                for (; i < toBeIncluded; i++)
                { new_array[i] = default(T); }
                for (i = toBeIncluded; i < new_size; i++)
                    new_array[i] = array[i - toBeIncluded];
            }
        }
        else if (new_size < previous_size)
        {
            if (addOrremoveFromTop)
            {
                int i;
                for (i = 0; i < new_size; i++)
                    new_array[i] = array[i];
                for (i = new_size; i < previous_size; i++)
                    array[i] = default(T);
            }
            else
            {
                int toBeExcluded = previous_size - new_size;
                int i;
                for (i = 0; i < toBeExcluded; i++)
                    array[i] = default(T);
                for (i = toBeExcluded; i < previous_size; i++)
                    new_array[i - toBeExcluded] = array[i];
            }
        }
        array = new_array;
    }
    public void growTop(int new_blocks)
    {

        T[] new_array = new T[new_blocks + length];
        for (int i = 0; i < length; i++)
            new_array[i] = array[i];
        for (int i = 0; i < new_blocks; i++)
            new_array[i + length] = default(T);
        array = new_array;
    }
    public void growBottom(int new_blocks)
    {
        T[] new_array = new T[new_blocks + length];
        for (int i = 0; i < new_blocks; i++) new_array[i] = default(T);
        for (int i = 0; i < length; i++)
            new_array[i + new_blocks] = array[i];
        array = new_array;
    }
    //public void trimTop(int shrink_blocks)
    //{
    //    int new_size = length - shrink_blocks; 
    //    T[] new_array = new T[new_size];
    //    for (int i = 0; i < new_size; i++)
    //        new_array[i] = array[i]; 
    //}
    //public void trimBottom(int shrink_blocks)
    //{
    //    int new_size = length - shrink_blocks;
    //    T[] new_array = new T[new_size];
    //    for (int i = 0; i < new_size; i++)
    //        new_array[i] = array[i + shrink_blocks];
    //}
    public void SetValue(int index, T value)
    {
        if (index > length - 1)
        {
            CLogger.err("Out of range of array size");
            return;
        }
        array[index] = value;
    }
    public T GetValue(int index) { return array[index]; }
    public T[] ToArray() { return array; }
}