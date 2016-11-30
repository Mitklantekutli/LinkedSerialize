# LinkedSerialize
Task:
–еализуйте функции сериализации и десериализации двусв€зного списка, заданного следующим

образом:

class ListNode

{

    public ListNode Prev;

    public ListNode Next;

    public ListNode Rand; // произвольный элемент внутри списка

    public string Data;

}

class ListRand

{

    public ListNode Head;
    
    public ListNode Tail;

    public int Count;

    public void Serialize(FileStream s){}

    public void Deserialize(FileStream s){}

}

ѕримечание: сериализаци€ подразумевает сохранение и восстановление полной структуры списка,

включа€ взаимное соотношение его элементов между собой.

“ест нужно выполнить без использовани€ библиотек/стандартных средств сериализации.