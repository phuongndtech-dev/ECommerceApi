using System;
using System.Text.RegularExpressions;

namespace ECommerceApi.Domain.ValueObjects
{
    public class Email : IEquatable<Email>
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));

            if (!IsValid(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.ToLowerInvariant();
        }

        public static bool IsValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
        }

        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string email) => new Email(email);

        public bool Equals(Email? other)
        {
            return other is not null && Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Email);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString() => Value;

        public static bool operator ==(Email? left, Email? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Email? left, Email? right)
        {
            return !Equals(left, right);
        }
    }
}