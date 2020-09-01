![NekoVM](https://user-images.githubusercontent.com/13326808/91330033-927bf600-e7d1-11ea-81a3-18be0ca9a065.png)

<!-- Name -->
<h1 align="center">
  Neko Virtual Machine for C# 😺
</h1>

<h4 align="center">
  For VM docs see <a href="http://nekovm.org/">NekoVM</a>
</h4>



<p align="center">
  <a href="#">
    <img alr="MIT License" src="https://img.shields.io/:license-MIT-blue.svg">
  </a>
  <a href="https://t.me/ivysola">
    <img alt="Telegram" src="https://img.shields.io/badge/Ask%20Me-Anything-1f425f.svg">
  </a>
</p>
<p align="center">
  <a href="#">
    <img src="https://forthebadge.com/images/badges/made-with-c-sharp.svg">
    <img src="https://forthebadge.com/images/badges/ages-18.svg">
    <img src="https://forthebadge.com/images/badges/powered-by-water.svg">
  </a>
</p>


## 🧬 Roadmap

- [ ] Documentation
- [ ] Improved API
- [x] Marshaling default types (without `vabstract` and `vkind`)
- [ ] Marshaling POCO object
- [ ] Marshaling neko std types
- [ ] Marshaling non-primitive .NET types
- [ ] More tests
- [ ] Define modules on runtime
- [ ] Improved Thread support
- [ ] Remove binaries from repo (automatization build for this)
- [x] dotnet global tools neko compiler
    - [x] win-x64
    - [x] osx-x64 (need help with testing)
    - [x] linux-x64

### 💫 Fast start

```csharp
using var vm = new Neko();
```

### Load *.n modules

```csharp
var module = vm.LoadModule(new FileInfo("module.n"));
```


### Get exports from module


```haxe
// module.n
$exports.GetInt = function() {
    return 42;
}
```

```csharp
using var vm = new Neko();
var module = vm.LoadModule(new FileInfo("module.n"));
Console.WriteLine(module["GetInt"].Invoke()); // NekoInt32 { Value: 42 }
```


### Threading

Each time you are going to use a VM API outside of the thread in which it was created, use the following code

```csharp
vm.GuardBarrier()
```

Also, `vm.ThreadID` id managed thread in which it was created (see `Thread.CurrentThread.ManagedThreadId`)


### Install compiler

```poweshell
dotnet tool install --global dotnet-nekoc --version 2.3.0-preview.2
```
