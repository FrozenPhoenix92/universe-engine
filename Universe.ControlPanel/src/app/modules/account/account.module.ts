import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { ButtonModule } from "primeng/button";
import { MessageModule } from "primeng/message";
import { MessagesModule } from "primeng/messages";

import { UnauthorizedGuard } from "../../guards/unauthorized.guard";
import { SharedModule } from "../../shared.module";
import { AccountComponent } from "./account.component";
import { SignInComponent } from "./pages/sign-in.component";
import { SignUpComponent } from "./pages/sign-up.component";
import { EmailConfirmationComponent } from "./pages/email-confirmation.component";
import { ForgotPasswordComponent } from "./pages/forgot-password.component";
import { ResetPasswordComponent } from "./pages/reset-password.component";


@NgModule({
	declarations: [
		AccountComponent,
		SignInComponent,
		SignUpComponent,
		EmailConfirmationComponent,
		ForgotPasswordComponent,
		ResetPasswordComponent
	],
	imports: [
		CommonModule,
		FormsModule,
		ReactiveFormsModule,

		InputTextModule,
		PasswordModule,
		ButtonModule,
		MessageModule,
		MessagesModule,

		SharedModule,
		RouterModule.forChild([
			{
				path: "",
				component: AccountComponent,
				canActivate: [ UnauthorizedGuard ],
				canActivateChild: [ UnauthorizedGuard ],
				children: [
					{ path: "", redirectTo: "sign-in", pathMatch: "full" },
					{ path: "sign-in", component: SignInComponent },
					{ path: "sign-up", component: SignUpComponent },
					{ path: "email-confirmation", component: EmailConfirmationComponent },
					{ path: "forgot-password", component: ForgotPasswordComponent },
					{ path: "reset-password", component: ResetPasswordComponent }
				]
			}
		])
	],
	schemas: [ CUSTOM_ELEMENTS_SCHEMA ]
})
export class AccountModule {}