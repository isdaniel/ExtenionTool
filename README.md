# ExtenionTool

There are some extension methods I had used and share for you

----

## `GetInstancesByAssembly<T>`

Get all instances which are `T` type from Assembly.

It will return `IEnumerable<T>` instances list.

Here is a sample

    Assembly.GetExecutingAssembly().GetInstancesByAssembly<ClassBase>();




