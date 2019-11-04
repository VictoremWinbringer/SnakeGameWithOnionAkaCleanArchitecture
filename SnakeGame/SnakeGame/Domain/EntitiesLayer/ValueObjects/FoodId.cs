using System;

class FoodId : BaseId<Guid>
{
    public FoodId(Guid value) : base(value)
    {
    }
}
