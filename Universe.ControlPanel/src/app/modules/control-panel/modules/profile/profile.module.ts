import { NgModule } from "@angular/core";
import { RouterModule } from "@angular/router";

import { CardModule } from "primeng/card";
import { TabMenuModule } from "primeng/tabmenu";
import { ButtonModule } from "primeng/button";
import { PasswordModule } from "primeng/password";
import { InputTextModule } from "primeng/inputtext";

import { ProfileComponent } from "./profile.component";
import { ProfileResolver } from "./profile.resolver";
import { PersonalComponent } from "./pages/personal.component";
import { AccessComponent } from "./pages/access.component";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";


@NgModule({
	declarations: [
		AccessComponent,
		PersonalComponent,
		ProfileComponent
	],
	imports: [
		FormsModule,
		ReactiveFormsModule,

		CardModule,
		TabMenuModule,
		ButtonModule,
		PasswordModule,
		InputTextModule,

		RouterModule.forChild([
			{
				path: "",
				component: ProfileComponent,
				resolve: { profilePersonal: ProfileResolver },
				children: [
					{ path: "", redirectTo: "personal", pathMatch: "full" },
					{ path: "personal", component: PersonalComponent },
					{ path: "access", component: AccessComponent }
				]
			}
		])
	],
	providers: [ ProfileResolver ]
})
export class ProfileModule {}