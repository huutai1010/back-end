﻿using FluentValidation;

namespace BLL.DTOs.Place.PlaceItem
{
    public class ItemDescriptionAddDto
    {
        public string? LanguageCode { get; set; }
        public string? NameItem { get; set; }
    }

    public class ItemDescriptionAddDtoValidator : AbstractValidator<ItemDescriptionAddDto>
    {
        public ItemDescriptionAddDtoValidator()
        {
            //RuleFor(a => a.LanguageCode).NotEmpty().WithMessage("LanguageCode can't be blank!");
            RuleFor(a => a.LanguageCode).MaximumLength(10).WithMessage("LanguageCode length is not greater than 10!");
            //RuleFor(a => a.NameItem).NotEmpty().WithMessage("NameItem can't be blank!");
            RuleFor(a => a.NameItem).MaximumLength(50).WithMessage("NameItem length is not greater than 50!");
        }
    }
}