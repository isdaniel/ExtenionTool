[![Build status](https://ci.appveyor.com/api/projects/status/58foeuvgq403bls5/branch/master?svg=true)](https://ci.appveyor.com/project/isdaniel/exteniontool/branch/master)

# ExtenionTool

There are some extension methods I had used and share for you

----

## `GetInstancesByAssembly<T>`

Get all instances which are `T` type from Assembly.

It will return `IEnumerable<T>` instances list.

Here is a sample

    Assembly.GetExecutingAssembly().GetInstancesByAssembly<ClassBase>();



## `AddAutoMapperProfileFromAssembly`


Register AutoMapper `Profile` class from assemblies.

Here is a sample

```
ContainerBuilder builder = new ContainerBuilder();
builder.AddAutoMapperProfileFromAssembly(typeof(AuditLogProfile).Assembly,...);
```



