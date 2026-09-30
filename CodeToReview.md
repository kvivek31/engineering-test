###  I used ChatGPT as a supporting tool during the code review to help identify potential issues and validate some of my observations. Some issues were identified independently through my own review, while ChatGPT helped surface additional issues and considerations that I then reviewed and verified myself. I also used ChatGPT to help organize and format the review document clearly.

# Code Review — CodeToReview.cs

## Summary

**Review Status: Request Changes**

There are a few correctness issues that should be addressed before merging. The main concerns are around random value generation, date calculations, age filtering, exception handling, and string truncation.

## Review Comments

###  1. Compilation Error — Line 2

```csharp
using System.Collegctions.Generic;
```

`Collegctions` is misspelled, so the code will not compile.

**Recommendation:**

```csharp
using System.Collections.Generic;
```

---

###  2. `Random.Next(0, 1)` Always Returns 0 — Line 43

```csharp
if (random.Next(0, 1) == 0)
```

The upper bound is exclusive, so this will always return `0`. As a result, `"Betty"` will never be generated.

**Recommendation:**

```csharp
var name = random.Next(2) == 0 ? "Bob" : "Betty";
```

---

###  3. `Random` Created Inside the Loop — Line 41

```csharp
var random = new Random();
```

Creating a new `Random` instance on every iteration is unnecessary and can produce poor/randomly repeated sequences.

**Recommendation:** Create one instance outside the loop or use `Random.Shared`.

---

###  4. Incorrect DOB Calculation — Line 50

```csharp
random.Next(18, 85) * 356
```

The code appears to treat a year as 356 days. This can generate incorrect dates of birth.

**Recommendation:**

```csharp
var age = random.Next(18, 85);
var dob = DateTime.UtcNow.AddYears(-age);
```

---

###  5. `olderThan30` Logic Is Reversed — Line 63

```csharp
x.DOB >= DateTime.Now.Subtract(...)
```

For someone older than 30, the DOB should be **before** the 30-year cutoff, not after it.

Also, the same incorrect 356-day calculation is used here.

**Recommendation:**

```csharp
var cutoffDate = DateTime.UtcNow.AddYears(-30);

return _people.Where(x =>
    x.Name == "Bob" &&
    x.DOB <= cutoffDate);
```

---

###  6. `Substring()` Result Is Ignored — Line 72

```csharp
(p.Name + " " + lastName).Substring(0, 255);
```

`string` is immutable, so `Substring()` returns a new string. Since the result is discarded, the name is not actually truncated.

**Recommendation:**

```csharp
var fullName = $"{p.Name} {lastName}";

return fullName.Length > 255
    ? fullName[..255]
    : fullName;
```

---

###  7. Exception Handling Loses the Original Exception — Lines 52–56

```csharp
catch (Exception e)
{
    throw new Exception("Something failed in user creation");
}
```

The original exception is discarded, making troubleshooting difficult. Also, there doesn't appear to be a reason to catch the exception here.

**Recommendation:** Remove the `try/catch`, or preserve the original exception as an inner exception.

---

###  8. Internal Collection Is Exposed — Line 59

```csharp
return _people;
```

This allows callers to directly modify the internal collection.

Consider returning `IReadOnlyList<People>` or a copy instead.

---

###  9. `GetPeople()` Accumulates State

Each call adds more people to `_people`.

For example:

```csharp
GetPeople(10);
GetPeople(10);
```

results in 20 people being stored/returned.

If the intention is to generate the requested number of people, consider returning a new collection from each call or separating generation from storage.

---

###  10. Naming and Date/Time Consistency

A few naming improvements would make the code easier to maintain:

* `People` → `Person`
* `DOB` → `DateOfBirth`
* `i` → `count`
* `_people` → `readonly`
* Avoid mixing `DateTime.Now`, `DateTime.UtcNow`, and `DateTimeOffset.UtcNow`.


