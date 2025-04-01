namespace SampleCrud.Exceptions
{
    public class CartItemNotFoundException : Exception
    {
        public CartItemNotFoundException() : base(String.Format("Item not found in cart.")) { }
        public CartItemNotFoundException(string? message) : base(message)
        {

        }
    }

    public class DuplicateUserException : Exception
    {
        public DuplicateUserException() : base(String.Format("User already exists.")) { }
        public DuplicateUserException(string? message) : base(message)
        {

        }
    }

    public class InvalidUserInfoException : Exception
    {
        public InvalidUserInfoException() : base(String.Format("Invalid user information.")) { }

        public InvalidUserInfoException(string message) : base(message)
        {

        }
    }

    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException() : base(String.Format("Item not found in orders.")) { }
        public OrderNotFoundException(string? message) : base(message)
        {

        }

    }

    public class PatchDocumentNotFoundException : Exception
    {
        public PatchDocumentNotFoundException() : base(String.Format("Patch Document unidentified.")) { }
        public PatchDocumentNotFoundException(string? message) : base(message)
        {
        }
    }

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException() : base(String.Format("Product not found.")) { }

        public ProductNotFoundException(string message) : base(message)
        {
        }


    }

    public class ReviewNotFoundException : Exception
    {
        public ReviewNotFoundException() : base(String.Format("Review not found.")) { }

        public ReviewNotFoundException(string message) : base(message)
        {
        }
    }

    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base(String.Format("User not found.")) { }

        public UserNotFoundException(string message) : base(message)
        {
        }
    }


}
