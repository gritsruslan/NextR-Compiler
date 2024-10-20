# NextR Programming Language Compiler

**NextR** is my own compiled programming language, created only for **educational purposes**. It is a **strongly typed language** with a **C-like syntax**, unrelated to the R programming language.

## Features:
- **Built-in data types:**  
  `bool`, `char`, `float`, `int`, `uint`,`double`, `string`, and `arrays`.  
- **Mathematical operations:**  
  `+`, `-`, `*`, `/`, `%`, `=`.  
- **Conditional operators:**  
  `==`, `<=`, `>=`, `>`, `<`, `!=`, `is`, `or`.  
- **Loops:**  
  `for`, `while`, and `loop`.  
- **Standard input/output** support:  
  `stdin`, `stdout`, `stderr`.  
- **User-defined functions:**  
  Create and call functions within the program.

---

## Compiler Overview

The compiler from scratch is being actively developed in **C# 12** on the **.NET 8 platform**. The **final architecture** of the compiler and the **target assembly language** for code generation are still under discussion.

### Planned Compilation Stages:

1. **Preprocessor**  
   Removes comments from the source code (single-line and multi-line).  
2. **Lexical analyzer**  
   Tokenizes the source code into meaningful lexical units (tokens).  
3. **Semantic analyzer** (optional)  
   Checks code for semantic correctness (e.g., type checking).  
4. **Optimizer** (optional)  
   Improves the performance of the generated code.  
5. **Code generator**  
   Converts the code into assembly or machine code.

### Currently Implemented Stages:

- **Preprocessor:**  
  Removes single-line and multi-line comments from the code.  
- **Lexical analyzer:**  
  Converts the source code into **tokens**.

---

## Example Program in NextR

```NextR

func IsPrime(int number) : bool {
	if(number <= 1)
		return false;
	for (int i = 2; i < number; i += 1) { 
		if (number % i == 0) 
			return false; 	
	} 
	return true;
}

func main() {
    stdoutPrintLine("Is number prime?");

    loop {
        stdoutPrintLine("Enter your number: ");
        
        var numberString = stdinGetLine();
        int number = numberString cast int;

        if (IsPrime(number))
            stdoutPrintLine("This number is prime!");
        else
            stdoutPrintLine("This number is compound!!");
        
        stdoutPrintLine();    
    }
}

```


---
## Privacy and Feedback

The project is distributed under the MIT license.
Feedback: gritsruslan@gmain .com
