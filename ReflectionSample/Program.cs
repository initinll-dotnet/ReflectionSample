using ReflectionMagic;

using ReflectionSample.Examples;

using System.Reflection;

Console.Title = "Learning Reflection";

var person = new Person("Kevin");

var privateField = person
    .GetType()
    .GetField(name: "_aPrivateField", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

privateField?.SetValue(person, "New private field value");

person.AsDynamic()._aPrivateField = "Updated value via ReflectionMagic";

//person.AsDynamic().MyMethod();
//person.AsDynamic().MyProperty = ...

Console.ReadLine();

static void Module_UsingReflection_For_InpectingMetadata()
{
    //string name = "Kevin";
    //var stringType = name.GetType();
    //var stringType = typeof(string);
    //Console.WriteLine(stringType);

    var currentAssembly = Assembly.GetExecutingAssembly();
    var typesFromCurrentAssembly = currentAssembly.GetTypes();
    foreach (var type in typesFromCurrentAssembly)
    {
        Console.WriteLine(type.Name);
    }

    var oneTypeFromCurrentAssembly = currentAssembly.GetType("ReflectionSample.Examples.Person");

    foreach (var constructor in oneTypeFromCurrentAssembly.GetConstructors())
    {
        Console.WriteLine(constructor);
    }

    foreach (var method in oneTypeFromCurrentAssembly
        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic))
    {
        Console.WriteLine($"{method}, public: {method.IsPublic}");
    }

    foreach (var field in oneTypeFromCurrentAssembly
        .GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
    {
        Console.WriteLine(field);
    }


    //var externalAssembly = Assembly.Load("System.Text.Json");
    //var typesFromExternalAssembly = externalAssembly.GetTypes();
    //var oneTypeFromExternalAssembly = externalAssembly.GetType("System.Text.Json.JsonProperty");

    //var modulesFromExternalAssembly = externalAssembly.GetModules();
    //var oneModuleFromExternalAssembly = externalAssembly.GetModule("System.Text.Json.dll");

    //var typesFromModuleFromExternalAssembly = oneModuleFromExternalAssembly?.GetTypes();
    //var oneTypeFromModuleFromExternalAssembly = oneModuleFromExternalAssembly
    //    .GetType("System.Text.Json.JsonProperty");

}

static void Module_UsingReflection_For_InstantiatingAndManipulatingObjects()
{
    var personType = typeof(Person);

    // get constructors info
    var personConstructors = personType
        .GetConstructors(
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    foreach (var personConstructor in personConstructors)
    {
        Console.WriteLine(personConstructor);
    }

    // get person constructor info
    var privatePersonConstructor = personType
        .GetConstructor(
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(string), typeof(int)], // should match constructor parameter types
            modifiers: null);

    // invoking constructor - public Person()
    var person1 = personConstructors[0].Invoke(parameters: null);

    // invoking constructor - public Person(string name)
    var person2 = personConstructors[1].Invoke(parameters: new object[] { "Kevin" });

    // invoking constructor - private Person(string name, int age)
    var person3 = personConstructors[2].Invoke(parameters: new object[] { "Kevin", 40 });

    var person33 = Activator
        .CreateInstance(typeof(Person));

    // invoking  constructor dynamically by Name
    // invoking constructor - public Person()
    var person4 = Activator
        .CreateInstance(
            assemblyName: "ReflectionSample",
            typeName: "ReflectionSample.Examples.Person")
        ?.Unwrap();

    // invoking constructor - public Person(string name)
    var person5 = Activator
        .CreateInstance(
            assemblyName: "ReflectionSample",
            typeName: "ReflectionSample.Examples.Person",
            ignoreCase: true,
            bindingAttr: BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { "Kevin" },
            culture: null,
            activationAttributes: null);

    var personTypeFromString = Type.GetType("ReflectionSample.Examples.Person");

    var person6 = Activator
        .CreateInstance(
            type: personTypeFromString,
            args: new object[] { "Kevin" });

    // invoking constructor - private Person(string name, int age)
    var person7 = Activator
        .CreateInstance(
            assemblyName: "ReflectionSample",
            typeName: "ReflectionSample.Examples.Person",
            ignoreCase: true,
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { "Kevin", 40 },
            culture: null,
            activationAttributes: null);

    // invoking constructor - using Assembly
    var assembly = Assembly.GetExecutingAssembly();
    var person8 = assembly.CreateInstance("ReflectionSample.Examples.Person");

    // Working with an object through Interfaces

    // create a new instance of a configured type
    var actualTypeFromConfiguration = Type.GetType(GetTypeFromConfiguration());
    var iTalkInstance = Activator.CreateInstance(actualTypeFromConfiguration) as ITalk;
    iTalkInstance.Talk("Hello world!");

    // Working with an object through Interfaces

    dynamic dynamicITalkInstance = Activator.CreateInstance(actualTypeFromConfiguration);
    dynamicITalkInstance.Talk("Hello world!");

    var personForManipulation = Activator
        .CreateInstance(
            assemblyName: "ReflectionSample",
            typeName: "ReflectionSample.Examples.Person",
            ignoreCase: true,
            bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { "Kevin", 40 },
            culture: null,
            activationAttributes: null)
        ?.Unwrap();

    // getting and setting properties and fields

    // get Person Name property - public string Name { get; set; }
    var nameProperty = personForManipulation?.GetType().GetProperty("Name");
    // set Person Name property - public string Name { get; set; }
    nameProperty?.SetValue(personForManipulation, "Sven");

    // get Person age field - public int age;
    var ageField = personForManipulation?.GetType().GetField("age");
    // set Person age field - public int age;
    ageField?.SetValue(personForManipulation, 36);

    // get Person age field - private string _aPrivateField = "initial private field value";
    var privateField = personForManipulation?.GetType()
        .GetField("_aPrivateField", BindingFlags.Instance | BindingFlags.NonPublic);
    // set Person age field - private string _aPrivateField = "initial private field value";
    privateField?.SetValue(personForManipulation, "updated private field value");

    // set Person Name property - public string Name { get; set; }
    personForManipulation
        ?.GetType()
        .InvokeMember(
            name: "Name",
            invokeAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
            binder: null,
            target: personForManipulation,
            args: new[] { "Emma" });

    // set Person age field - private string _aPrivateField = "initial private field value";
    personForManipulation
        ?.GetType()
        .InvokeMember(
            name: "_aPrivateField",
            invokeAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField,
            binder: null,
            target: personForManipulation,
            args: new[] { "second update for private field value" });

    Console.WriteLine(personForManipulation);

    // invoking methods

    // get method - public void Talk(string sentence)
    var talkMethod = personForManipulation
        ?.GetType()
        .GetMethod("Talk");

    // invoke method - public void Talk(string sentence)
    talkMethod
        ?.Invoke(
            obj: personForManipulation, 
            parameters: new[] { "something to say" });

    // invoke method - protected void Yell(string sentence)
    personForManipulation
        ?.GetType()
        .InvokeMember(
            name: "Yell",
            invokeAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
            binder: null, 
            target: personForManipulation, 
            args: new[] { "something to yell" });
}

static void Module_UsingReflection_For_InstantiatingAndManipulatingObjects_NetworkMonitorExample()
{
    NetworkMonitor.BootstrapFromConfiguration();

    Console.WriteLine("Monitoring network... something went wrong.");

    NetworkMonitor.Warn();
}

static void CodeFromFourthModule()
{
    var myList = new List<Person>();
    Console.WriteLine(myList.GetType());

    var myDictionary = new Dictionary<string, int>();
    Console.WriteLine(myDictionary.GetType());

    var dictionaryType = myDictionary.GetType();

    foreach (var genericTypeArgument in dictionaryType.GenericTypeArguments)
    {
        Console.WriteLine(genericTypeArgument);
    }

    foreach (var genericArgument in dictionaryType.GetGenericArguments())
    {
        Console.WriteLine(genericArgument);
    }

    var openDictionaryType = typeof(Dictionary<,>);

    foreach (var genericTypeArgument in openDictionaryType.GenericTypeArguments)
    {
        Console.WriteLine(genericTypeArgument);
    }

    foreach (var genericArgument in openDictionaryType.GetGenericArguments())
    {
        Console.WriteLine(genericArgument);
    }

    var createdInstance = Activator.CreateInstance(typeof(List<Person>));
    Console.WriteLine(createdInstance.GetType());

    //var openResultType = typeof(Result<>);
    //var closedResultType = openResultType.MakeGenericType(typeof(Person));
    //var createdResult = Activator.CreateInstance(closedResultType);
    //Console.WriteLine(createdResult.GetType());

    var openResultType = Type.GetType("ReflectionSample.Examples.Result`1");
    var closedResultType = openResultType.MakeGenericType(Type.GetType("ReflectionSample.Examples.Person"));
    var createdResult = Activator.CreateInstance(closedResultType);
    Console.WriteLine(createdResult.GetType());

    var methodInfo = closedResultType.GetMethod("AlterAndReturnValue");
    Console.WriteLine(methodInfo);

    var genericMethodInfo = methodInfo.MakeGenericMethod(typeof(Employee));
    genericMethodInfo.Invoke(createdResult, new object[] { new Employee() });
}

static void IoCContainerExample()
{
    var iocContainer = new IoCContainer();

    iocContainer.Register<IWaterService, TapWaterService>();
    var waterService = iocContainer.Resolve<IWaterService>();

    //iocContainer.Register<IBeanService<Catimor>, ArabicaBeanService<Catimor>>();
    //iocContainer.Register<IBeanService<>, ArabicaBeanService<>>();
    //iocContainer.Register<typeof(IBeanService<>), typeof(ArabicaBeanService<>)>();
    iocContainer.Register(typeof(IBeanService<>), typeof(ArabicaBeanService<>));

    iocContainer.Register<ICoffeeService, CoffeeService>();
    var coffeeService = iocContainer.Resolve<ICoffeeService>();
}

static string GetTypeFromConfiguration()
{
    return "ReflectionSample.Examples.Person";
}