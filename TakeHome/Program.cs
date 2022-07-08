// See https://aka.ms/new-console-template for more information
using DataSource;
using InventoryAllocator;

//Console.WriteLine("Hello, World!");
//var facade = new InventoryAllocatorFacade();
//var order = "{\"Header\":1, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 5}, {\"Product\":\"B\", \"Quantity\": 5}, {\"Product\":\"C\", \"Quantity\": 5}, {\"Product\":\"D\", \"Quantity\": 5}, {\"Product\":\"E\", \"Quantity\": 5} ]}";
//var order1 = "{\"Header\":1, \"Lines\": [{\"Product\":\"B\", \"Quantity\": 1}]}";
//var order2 = "{\"Header\":2, \"Lines\": [{\"Product\":\"A\", \"Quantity\": 1}]}";
//facade.AddOrder(order);
//facade.AddOrder(order2);


var dataSource = new Datasource();
dataSource.RunOrderStreams();