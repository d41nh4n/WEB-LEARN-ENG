namespace Allen.Common;

public enum AccessTypeDeckEnum
{
    // user created (has CRUD role)
    Owner = 0,
    // user save deck to using (has R role)
    Shared = 1,
    // user clone deck to private = create new deck (has CRUD role) but not create this deck
    Cloned = 2
}
