using System.Collections;
using CardBattle;
using NUnit.Framework;
using UnityEngine.TestTools;

public class HealthStateTests {

	[Test]
	public void ZeroIntialization() {
		var h = new HealthState();
		Assert.That(h.health == 0);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void ValueInitialization() {
		var h = new HealthState(){health = 20};
		Assert.That(h.health == 20);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void TemporaryDamageNegation() {
		var h = new HealthState(){health = 20, temporaryDamageReduction = 20};
		h.ApplyDamage(20);
		Assert.That(h.health == 20);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void PermanentDamageNegation() {
		var h = new HealthState(){health = 20, permanentDamageReduction = 20};
		h.ApplyDamage(20);
		Assert.That(h.health == 20);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void MixedDamageNegation() {
		var h = new HealthState(){health = 20, temporaryDamageReduction = 10, permanentDamageReduction = 10};
		h.ApplyDamage(20);
		Assert.That(h.health == 20);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void MixedDamageNegationHealthDamage() {
		var h = new HealthState(){health = 20, temporaryDamageReduction = 10, permanentDamageReduction = 10};
		h.ApplyDamage(30);
		Assert.That(h.health == 10);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
	
	[Test]
	public void Healing() {
		var h = new HealthState(){health = 20, temporaryDamageReduction = 10, permanentDamageReduction = 10};
		h.ApplyHealing(10);
		Assert.That(h.health == 30);
		Assert.That(h.temporaryDamageReduction == 10);
		Assert.That(h.permanentDamageReduction == 10);
	}
	
	[Test]
	public void NegativeHealing() {
		var h = new HealthState(){health = 20, temporaryDamageReduction = 10, permanentDamageReduction = 10};
		h.ApplyHealing(-30);
		Assert.That(h.health == 10);
		Assert.That(h.temporaryDamageReduction == 0);
		Assert.That(h.permanentDamageReduction == 0);
	}
}