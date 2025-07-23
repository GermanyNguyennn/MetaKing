using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaKing.ViewModels.Contents
{
    public class BrandCreateRequestValidator : AbstractValidator<BrandCreateRequest>
    {
        public BrandCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(string.Format(Messages.Required, "Tên thương hiệu"));
        }
    }
}
