﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Composition;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ICSharpCode.NRefactory6.CSharp.GenerateMember.GenerateDefaultConstructors
{
	public class CSharpGenerateDefaultConstructorsService : AbstractGenerateDefaultConstructorsService<CSharpGenerateDefaultConstructorsService>
	{
		protected override bool TryInitializeState(
			SemanticDocument document, TextSpan textSpan, CancellationToken cancellationToken,
			out SyntaxNode baseTypeNode, out INamedTypeSymbol classType)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				var node = document.Root.FindToken(textSpan.Start).GetAncestor<TypeSyntax>();
				if (node != null)
				{
					if (node.Parent is BaseTypeSyntax && node.Parent.IsParentKind(SyntaxKind.BaseList))
					{
						var baseList = (BaseListSyntax)node.Parent.Parent;
						if (baseList.Types.Count > 0 &&
							baseList.Types[0].Type == node &&
							baseList.IsParentKind(SyntaxKind.ClassDeclaration))
						{
							var semanticModel = document.SemanticModel;
							classType = semanticModel.GetDeclaredSymbol(baseList.Parent, cancellationToken) as INamedTypeSymbol;
							baseTypeNode = node;
							return classType != null;
						}
					}
				}
			}

			baseTypeNode = null;
			classType = null;
			return false;
		}
	}
}