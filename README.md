# LinkedSerialize
Task:
���������� ������� ������������ � �������������� ����������� ������, ��������� ���������

�������:

class ListNode

{

    public ListNode Prev;

    public ListNode Next;

    public ListNode Rand; // ������������ ������� ������ ������

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

����������: ������������ ������������� ���������� � �������������� ������ ��������� ������,

������� �������� ����������� ��� ��������� ����� �����.

���� ����� ��������� ��� ������������� ���������/����������� ������� ������������.