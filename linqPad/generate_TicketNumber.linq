<Query Kind="Program" />

void Main()
{
	string ticketNumber= "UTIM-5356";
	int startNumber = 1;
	int endNumber = 5;
	var list = Enumerable.Range(startNumber, endNumber).Select(x => $"\"{ticketNumber}_{x:00}\"");
    string.Join(",", list).Dump();
}

// Define other methods and classes here