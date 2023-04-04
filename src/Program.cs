/*
    Clean Code - Chapter 17: Smells and Heuristics - Functions

    F1: Too Many Arguments
    F2: Output Arguments
    F3: Flag Arguments
    F4: Dead Function
*/

public class Helpers
{
    public bool Random()
    {
        return new Random().Next(2) == 0;
    }
}
public class Program
{
    private void DisplayHeader()
    {
        string header = "*** A very sophisticated student grades dashboard ***";
        int padding = (Console.WindowWidth + header.Length) / 2;

        header = header.PadLeft(padding);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(header);
    }
    public static void Main(string[] args)
    {
        new Program().start();
    }

    private void start()
    {
        DisplayHeader();
        var grades = GetMockgrades();
        var students = grades
          .Select(grade => grade.Student)
          .Distinct()
          .ToList();

        string message = "";
        string name = "";

        foreach (var student in students)
        {
            name = student.DisplayFullName ? student.FullName : student.ProperName;
            message = $"Student: {name}\n";

            var studentGrades = grades
                .Where(grade => grade.Student == student)
                .ToList();
            
            foreach (var grade in studentGrades)
            {
                var padding = new String(' ', grade.MaxSubjectLength - grade.Subject.ToString().Length);
                message += $"\tSubject: {grade.Subject}{padding} - Grade {grade.FormattedGrade}\n";
            }

            DisplayMessage(message);
        }
        Console.Write("\n\n\nPress any key to close...");
        Console.ReadKey();
    }

    private static void DisplayMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
    }

    #region Mocks
    private static Grade[] GetMockgrades()
    {
        var helper = new Helpers();

        return new List<Student>() {
            new Student("Cleo", "Strong", displayFullName: helper.Random()),
            new Student("Olivia", "Allen", displayFullName: helper.Random()),
            new Student("Fred", "Cisneros", displayFullName: helper.Random()),
            new Student("Julia", "Pacheco", displayFullName: helper.Random()),
            new Student("Gene", "Dixon", displayFullName: helper.Random())
        }
        .Select(student =>
        {
            return Enum.GetValues(typeof(Subject))
                .Cast<Subject>()
                .Select(subject => new Grade(
                  student,
                  subject,
                  value: new Random().Next(5, 11),
                  displayLetterGrade: helper.Random())
                )
                .ToArray();
        })
        .SelectMany(x => x)
        .ToArray();
    }
    #endregion
}

#region types
public enum Subject
{
    Math,
    Coding,
    Science
}

public enum LetterGrade
{
    A = 10,
    B = 9,
    C = 8,
    D = 7,
    E = 6,
    F = 5
}

public class Student
{
    public Student(string firstName, string lastname, bool displayFullName = false)
    {
        FirstName = firstName;
        LastName = lastname;
        DisplayFullName = displayFullName;
    }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool DisplayFullName { get; private set; }
    public string ProperName
    {
        get
        {
            return $"{LastName}, {FirstName}";
        }
    }
    public string FullName
    {
        get
        {
            return $"{FirstName}, {LastName}";
        }
    }
}

public class Grade
{
    private const string INVALID = $"!INVALID";
    public Grade(Student student, Subject subject, int value, bool displayLetterGrade = false)
    {
        Student = student;
        Subject = subject;
        Value = value;
        DisplayLetterGrade = displayLetterGrade;
    }
    public Student Student { get; private set; }
    public Subject Subject { get; private set; }
    public int Value { get; private set; }
    public bool DisplayLetterGrade { get; private set; }
    public string LetterValue
    {
        get
        {
            return (Value >= 5 && Value <= 10) ? ((LetterGrade)Value).ToString() : INVALID;
        }
    }
    public int MaxSubjectLength
    {
        get
        {
            return Enum.GetValues(typeof(Subject))
              .Cast<Subject>()
              .Select(subject => subject.ToString())
              .ToList()
              .Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur)
              .Length;
        }
    }
    public string FormattedGrade
    {
        get
        {
            return DisplayLetterGrade ? LetterValue : Value.ToString();
        }
    }
}
#endregion
