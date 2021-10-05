using System.Collections.Generic;

public class Stack<T>
{
    //The stack uses a linked list in order to hold all the elements in the stack
    //The pointer Top is used to identify the index of the last element in the stack
    //There are four public functions: Push, Pop, Peek, Clear

    protected Linked_List<T> StackList;
    protected int Top;
    public int Count { get; private set; }

    public Stack()
    {
        //Initiate variables
        Top = -1;
        Count = 0;
        StackList = new Linked_List<T>();
    }

    /// <summary>
    /// Removes top item
    /// </summary>
    /// <returns></returns>
    public T Pop()
    {
        if (!IsEmpty())
        {
            //Set pointer down by one and decrease size
            //Return top item
            Count--;
            return StackList.RemoveAt(Top--);
        }
        return default;
    }

    /// <summary>
    /// Adds a new item to the stack
    /// </summary>
    /// <param name="Item"></param>
    /// <returns></returns>
    public void Push(T Item)
    {
        //Set pointer up by one and increase size
        //Add item
        Top++;
        StackList.Add(Item);
        Count++;
    }

    /// <summary>
    /// Returns the top item of the stack
    /// </summary>
    /// <returns></returns>
    public T Peek()
    {
        //Check stack is not empty
        if (!IsEmpty())
            //Return item
            return StackList[Top];
        return default;
    }

    /// <summary>
    /// Clears the stack
    /// </summary>
    public void Clear()
    {
        //Reset list, pointer and size
        Top = -1;
        Count = 0;
        StackList = new Linked_List<T>();
    }

    /// <summary>
    /// If pointer set to -1
    /// </summary>
    /// <returns></returns>
    private bool IsEmpty()
    {
        //Check if pointer is -1 to determine if stack is empty
        if (Top == -1)
            return true;
        return false;
    }
}


/// <summary>
///Linked_List holds an unfixed number of elements.
///The data type can be specified at instantiation (type T).
///Public functions: Find, Add, Remove, RemoveAt, PrintList.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Linked_List<T>
{
    //Linked_List holds an unfixed number of elements
    //The data type can be specified at instantiation (type T)
    //Public functions: Find, Add, Remove, RemoveAt, PrintList

    //Nodes references to aid searching
    private Node<T> Start;
    private Node<T> End;

    //Custom indexing
    //Uses two private functions, FindByIndex and SetByIndex as getter and setter of nodes
    public T this[int index]
    {
        get
        {
            return FindByIndex(index);
        }
        set
        {
            SetByIndex(index, value);
        }
    }

    /// <summary>
    /// Prints the data in the list to console for debugging purposes
    /// </summary>
    public void PrintList() 
    {
        string completestr = "";
        Node<T> CurrentNode = Start;
        //Iterates through list and creates string of data
        while (CurrentNode != null)
        {
            completestr += CurrentNode.GetData().ToString() + "\n";
            CurrentNode = CurrentNode.GetNext();
        }
        //Prints data string to console
        UnityEngine.Debug.Log("\n" + completestr + "\n");
    }

    /// <summary>
    /// Returns node with matching data of parameter
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    public Node<T> Find(T Data)
    {
        Node<T> CurrentNode = Start;
        //Iterates over the list and checks data does not equal that stored in each node
        while (!EqualityComparer<T>.Default.Equals(CurrentNode.GetData(), Data))
        {
            //Continue to next node
            CurrentNode = CurrentNode.GetNext();
            //Return default if data is not found
            if (CurrentNode == null)
                return default;
        }

        //Return node if found
        return CurrentNode;
    }

    /// <summary>
    /// Add new node
    /// </summary>
    /// <param name="newData"></param>
    public void Add(T newData)
    {
        //Create new node with data of newData
        Node<T> NewNode = new Node<T>();
        NewNode.SetData(newData);
        //If list is empty, make new node the start and end
        if (Start == null)
        {
            Start = NewNode;
            End = NewNode;
            return;
        }

        //Set reference to new node on last node
        End.SetNext(NewNode);
        End = NewNode;
    }

    /// <summary>
    /// Remove node matching data of parameter
    /// </summary>
    /// <param name="Data"></param>
    /// <returns></returns>
    public T Remove(T Data)
    {
        //Track current and previous node
        Node<T> CurrentNode = Start;
        Node<T> PreviousNode = Start;
        //Iterate over list while Data does not equal that in each node
        while (!EqualityComparer<T>.Default.Equals(CurrentNode.GetData(), Data))
        {
            //Return null if not found
            if (CurrentNode.GetNext() == null)
                return default;

            //Continue to next node
            PreviousNode = CurrentNode;
            CurrentNode = CurrentNode.GetNext();
        }

        //Set previous node's next to node ahead of current node
        PreviousNode.SetNext(CurrentNode.GetNext());
        //If start is removed, set start to second node
        if (CurrentNode == Start)
            Start = Start.GetNext();
        //If end is removed, set end to second last node
        if (CurrentNode.GetNext() == null)
            End = PreviousNode;

        //Return removed value
        return CurrentNode.GetData();
    }

    public T RemoveAt(int index)
    {
        //Call remove method with parameter of element at given index 
        return Remove(this[index]);
    }

    private T FindByIndex(int index)
    {
        Node<T> CurrentNode = Start;

        //Iterate over nodes until index reached
        for (int i = 0; i < index; i++)
        {
            CurrentNode = CurrentNode.GetNext();

            //If index not in list, throw exception
            if (CurrentNode == null)
                throw new System.ArgumentException("Index out of bounds");
        }
        //Return data of node at index
        return CurrentNode.GetData();
    }

    private void SetByIndex(int index, T data)
    {
        Node<T> CurrentNode = Start;

        //Iterate over nodes until index reached
        for (int i = 0; i < index; i++)
        {
            CurrentNode = CurrentNode.GetNext();
            //If index not in list, throw exception
            if (CurrentNode.GetNext() == null)
                throw new System.ArgumentException("Index out of bounds");
        }
        //Set data of node at index
        CurrentNode.SetData(data);
    }
}

/// <summary>
/// Node holds the data of each element in a Linked_List
/// </summary>
public class Node<T>
{
    // Data holds data to be stored of type T
    // Next holds reference to next node in the list
    private T Data;
    private Node<T> Next;

    //Custom getters and setters of Data
    public T GetData()
    {
        return Data;
    }

    public void SetData(T newData)
    {
        Data = newData;
    }

    //Custom getters and setters of Next
    public Node<T> GetNext()
    {
        return Next;
    }

    public void SetNext(Node<T> NextNode)
    {
        Next = NextNode;
    }
}